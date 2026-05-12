using System.Collections;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Server;
using Assets.Scripts.Zones;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class AgoraConnectionManager : NetworkBehaviour
{
    private AgoraVoice _agoraVoice;
    private ServerVerification _serverVerification;
    private string _customChannelName;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            return;
        }

        _agoraVoice = FindObjectOfType<AgoraVoice>();
        if (_agoraVoice == null)
        {
            return;
        }

        _serverVerification = FindObjectOfType<ServerVerification>();
        if (_serverVerification != null)
        {
            _serverVerification.LocalClientAccessGranted += ConnectToAgora;
        }
        else
        {
            ConnectToAgora();
        }
    }

    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();

        if (_agoraVoice != null)
        {
            _agoraVoice.LeaveChannel();
        }
        if (_serverVerification != null)
        {
            _serverVerification.LocalClientAccessGranted -= ConnectToAgora;
        }
        base.OnNetworkDespawn();
    }

    public async UniTask SetCustomAudioChannel(string audioChannelName)
    {
        await UniTask.WaitUntil(() => _agoraVoice != null && !string.IsNullOrEmpty(_agoraVoice.CurrentAudioChannelShortName));
        
        _customChannelName = audioChannelName;
        if (_agoraVoice.CurrentAudioChannelShortName == _customChannelName)
        {
            Debug.Log($"Agora. Already connected to channel: {_agoraVoice.CurrentAudioChannelShortName}");
            return;
        }

        _agoraVoice.JoinChannel(audioChannelName);

        await UniTask.WaitUntil(() => _agoraVoice.CurrentAudioChannelShortName == audioChannelName);
        Debug.Log($"Agora. Joined to channel: {_agoraVoice.CurrentAudioChannelShortName}");
    }

    public async UniTask SetDefaultAudioChannel()
    {
        await UniTask.WaitUntil(() => _agoraVoice != null && !string.IsNullOrEmpty(_agoraVoice.CurrentAudioChannelShortName));
        
        _customChannelName = null;
        if (_agoraVoice.CurrentAudioChannelShortName == _agoraVoice.channelName)
        {
            Debug.Log($"Agora. Already connected to channel: {_agoraVoice.CurrentAudioChannelShortName}");
            return;
        }

        await UniTask.WaitUntil(() => _agoraVoice.CurrentAudioChannelShortName == _agoraVoice.channelName);
        Debug.Log($"Agora. Joined to channel: {_agoraVoice.CurrentAudioChannelShortName}");
    }

    private void ConnectToAgora()
    {
        ZonesModel zonesModel = FindObjectOfType<ZonesModel>();
        if (zonesModel != null)
        {
            StartCoroutine(AudioChannelChangeRoutine(zonesModel));
        }
        else
        {
            _agoraVoice.JoinDefaultChannel();
        }
    }

    private IEnumerator AudioChannelChangeRoutine(ZonesModel zonesModel)
    {
        string currentChannelName = string.Empty;
        while (true)
        {
            yield return null;

            ZoneController currentZoneController = zonesModel.ZoneController;

            string newChannelName;
            if (!string.IsNullOrEmpty(_customChannelName))
            {
                newChannelName = _customChannelName;
            }
            else if (currentZoneController != null && currentZoneController.IsMuted)
            {
                newChannelName = currentZoneController.PrivateChannelName;
            }
            else
            {
                newChannelName = _agoraVoice.channelName;
            }

            if (!newChannelName.Equals(currentChannelName) && !_agoraVoice.IsBusy)
            {
                currentChannelName = newChannelName;
                _agoraVoice.JoinChannel(newChannelName);
            }
        }
    }
}
