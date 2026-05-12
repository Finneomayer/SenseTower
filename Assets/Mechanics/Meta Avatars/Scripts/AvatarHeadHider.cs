using System.Collections;
using System.Collections.Generic;
using Oculus.Skinning.GpuSkinning;
using Unity.XR.CoreUtils;
using UnityEngine;

public class AvatarHeadHider : MonoBehaviour
{
    private OvrAvatarGpuInterpolatedSkinnedRenderable _avatarHead = null;

    private void Update()
    {
        if (_avatarHead == null)
        {
            _avatarHead = GetComponentInChildren<OvrAvatarGpuInterpolatedSkinnedRenderable>();
            if (_avatarHead != null)
            {
                _avatarHead.gameObject.layer = LayerMask.NameToLayer("PlayerCameraDontSee");
                Debug.LogWarning($"SetLayer --------------------{_avatarHead.gameObject.GetHashCode()}");
            }
            else
            {
                Debug.LogWarning("Can't find");
            }
        }

        foreach (Transform child in transform)
        {
            child.gameObject.layer = 15;
        }



    }

    public void Debug_g(string text)
    {
        Debug.LogWarning(text);
    }


}
