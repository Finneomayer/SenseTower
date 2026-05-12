using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Avatar2;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

public enum EmojiType
{
    ThumbsUp,
    ThumbsDown,
    Like,
    Question,
    Smile,
    Friending
}

[Serializable]
public class EmojiEffect
{
    public EmojiType Type;
    public ParticleSystem Effect;
}

public class PlayerEmoji : NetworkBehaviour
{
    [SerializeField] private SampleAvatarEntity _headPosition;
    [SerializeField] private List<EmojiEffect> _emojis;
    [SerializeField] private EmojiSwitcher _watchSwitcher;
    [SerializeField] private AudioSource _pressSound;

    private EmojiEffect _currentEmojiEffect;

    private void Start()
    {
        if (_emojis.Count > 0)
        {
            _currentEmojiEffect = _emojis[0];
            StartCoroutine(RefreshPositionCoroutine());
            _watchSwitcher.OnPressEmoji += Switcher_OnPressEmoji;
        }
    }

    public void SetSwitcher(EmojiSwitcher switcher)
    {
        switcher.OnPressEmoji += Switcher_OnPressEmoji;
    }

    public void StartEmojiEffect(EmojiType type)
    {
        StartEmojiServerRpc(type);
    }

    public void StopEmojiEffect(EmojiType type)
    {
        StopEmojiServerRpc(type);
    }

    private void Switcher_OnPressEmoji(EmojiType type)
    {
        StartEmojiServerRpc(type);        
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEmojiServerRpc(EmojiType type)
    {
        StartEmojiClientRpc(type);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopEmojiServerRpc(EmojiType type)
    {
        StopEmojiClientRpc(type);
    }

    [ClientRpc]
    private void StartEmojiClientRpc(EmojiType type)
    {
        foreach (var e in _emojis)
        {
            if (e.Type == type)
            {
                if (!_pressSound.isPlaying) _pressSound.Play();
                if (!_currentEmojiEffect.Effect.isPlaying)
                {
                    _currentEmojiEffect = e;
                    e.Effect.Play();
                }
                break;
            }
        }
    }

    [ClientRpc]
    private void StopEmojiClientRpc(EmojiType type)
    {
        foreach (var e in _emojis)
        {
            if (e.Type == type)
            {
                if (e.Effect.isPlaying)
                {
                    e.Effect.Stop();
                }
                break;
            }
        }
    }

    private IEnumerator RefreshPositionCoroutine()
    {
        const float ForwardShiftFromCamera = 0.1f;
        const float HeigthAboveHeadCenter = 0.5f;

        WaitForSeconds avatarCheckDelay = new(0.1f);
        WaitForEndOfFrame positionRefreshDelay = new();

        Transform avatarHeadTransform = null;
        while (avatarHeadTransform == null)
        {
            avatarHeadTransform = _headPosition.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
            yield return avatarCheckDelay;
        }

        while (true)
        {
            yield return positionRefreshDelay;

            Camera camera = Camera.main;
            if (camera == null)
            {
                continue;
            }

            //TODO: Delete, if we are shure that avatarHeadTransform can not become null;
            while (avatarHeadTransform == null)
            {
                avatarHeadTransform = _headPosition.GetSkeletonTransform(CAPI.ovrAvatar2JointType.Head);
                yield return avatarCheckDelay;
            }

            Vector3 camToHeadDirection = (avatarHeadTransform.position - camera.transform.position).normalized;
            camToHeadDirection.y = 0;
            if (camToHeadDirection == Vector3.zero)
            {
                camToHeadDirection = camera.transform.forward;
            }

            Vector3 newCanvasPosition = avatarHeadTransform.position + ForwardShiftFromCamera * camToHeadDirection;
            newCanvasPosition.y = avatarHeadTransform.transform.position.y + HeigthAboveHeadCenter;

            _currentEmojiEffect.Effect.transform.position = newCanvasPosition;
        }
    }
}
