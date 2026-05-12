using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.Player;
using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squirrel.SimpleSplat;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using Assets.Mechanics.Mafia;
using Assets.Scripts.Infrastructure.Factory;
using Assets.Scripts.Server;
using Assets.Scripts.Shared;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    public class AvatarRecorderController : NetworkBehaviour
    {
        [SerializeField] private int _recordLength = 60; //seconds
        [SerializeField] private float _recordPeriod = 0.1f; //seconds
        [SerializeField] private RecorderUserView _view;
        [SerializeField] private AudioSource _audioOutput;
        [SerializeField] private SampleAvatarEntity _targetAvatar;
        [SerializeField] private int DelayBuffer = 30;
        [SerializeField] private Transform _shiftObject;

        private SampleAvatarEntity _sourceAvatar;

        private static readonly float[] StreamLodSnapshotIntervalSeconds = new float[OvrAvatarEntity.StreamLODCount] { 1f / 72, 2f / 72, 3f / 72, 4f / 72 };
        private const string AndroidMicrophoneName = "Android audio input";
        private const string WindowsMicrophoneName = "Headset Microphone (Oculus Virtual Audio Device)";
        private string _currentMicrophoneName;
        private int _audioBufferToPlay = 0;
        private int _bufferToSend = 0;

        private MovementRecorder _recorderServer; //server
        private AvatarRecorderData _serverDataToSave; //server
        private List<AvatarFrame[]> _serverFramesToSave; //server
        private List<float> _serverAudioFloatToSave; //server
        private Coroutine _playingCoroutineServer;

        private Coroutine _recordingCoroutineClient; //client
        private Coroutine _tickPlaySoundCoroutineClient; //client

        private AvatarFrame[] _frameToRecord;
        private AvatarFrame[] _frameToPlay;
        private AudioClip _recordedClip;
        private AudioClip _playingClip;
        private int _playingPosition = 0;
        private float _currentEndTime;

        private CustomSnapTurnProvider _player;
        private Transform _playerTransfom;
        private Queue<AvatarFrame[]> _movementToPlay;

        private bool _isPlayingOnServer;
        private bool _isPausedOnServer;
        private bool _isPlayingOnClient;
        private bool _isBlockedWhilePauseClient;
        private AvatarRecorderData _openedDataOnServer;

        private IAvatarRecorderService _avatarRecorderService; //server
        private IServerApiData _serverApiData; //server

        private AvatarRecorderSaver _saverServer;
        private OnObjectData _thisObjectData;

        private void Awake()
        {
#if UNITY_SERVER
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _avatarRecorderService = commonDIInstaller.Resolve<IAvatarRecorderService>();
                _serverApiData = commonDIInstaller.Resolve<IServerApiData>();
            }
#endif
        }

        private void Start()
        {
#if UNITY_SERVER
            _saverServer = new AvatarRecorderSaver();
            _saverServer.Init(_avatarRecorderService, _serverApiData);
#endif
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
#if UNITY_SERVER
            Debug.Log("Recorder spawned on server");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedToServer;
#else
            Debug.LogWarning("OnNetworkSpawn");
            if (TryGetComponent(out OnObjectData objectData)) _thisObjectData = objectData;

            StartCoroutine(FindAvatarAndInit());           
#endif
        }

        private void FixedUpdate()
        {
#if UNITY_SERVER
            if (NetworkManager.ConnectedClients.Count == 0) StopPlayingOnServerRpc("");
#endif
        }

        public override void OnNetworkDespawn()
        {
#if UNITY_SERVER
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedToServer;
#endif
        }

        private IEnumerator FindAvatarAndInit()
        {
            NetworkPlayer[] users = new NetworkPlayer[] { };
            while (users.Length == 0)
            {
                users = FindObjectsOfType<NetworkPlayer>();
                yield return new WaitForSeconds(0.1f);
            }
            
            SampleAvatarEntity result = null;

            foreach (var user in users)
            {
                if (user.NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    while (result == null)
                    {
                        result = user.GetDefaultAvatar();
                        yield return new WaitForEndOfFrame();
                    }
                    
                    _playerTransfom = user.transform;
                    _player = user.GetComponent<CustomSnapTurnProvider>();
                    break;
                }
            }

            Debug.LogWarning(result == null);
            InitClient(result);

            if (IsOwner) _view.InitAsOwner(_playerTransfom);
            else _view.InitAsWatcher(_playerTransfom);
        }

        private void OnClientConnectedToServer(ulong id)
        {
            Debug.Log("new client connected");
            if (_isPlayingOnServer)
            {
                RecordParams param = _openedDataOnServer.AvatarParameters;
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { id }
                    }
                };

                PrepareAvatarClientRpc(param.AvatarNumber, param.Shift, clientRpcParams);

                var audioClip = _openedDataOnServer.Audio;
                StartPlayClientRpc(audioClip.name, audioClip.frequency * param.LengthInSeconds, audioClip.channels, audioClip.frequency, false, clientRpcParams);

                if (_isPausedOnServer) PausePlayClientRpc(true);
            }
        }

        public void InitClient(SampleAvatarEntity sourceAvatar)
        {
#if !UNITY_SERVER     
            _view.SetButtons(true, false);
            StartCoroutine(StartCheckRecordingExisting());

            _view.RequestToRecord += UserRequestToRecord;
            _view.RequestToStopRec += UserRequestToStopRecRecording;
            _view.RequestToPlay += UserRequestToPlay;
            _view.RequestToStopPlay += UserRequestToStopPlaying;
            _view.RequestToPausePlay += UserRequestToPausePlaying;

            _sourceAvatar = sourceAvatar;
            //_targetAvatar.ReloadAvatarManually(AvatarSessionData.AvatarAssetId.Value.ToString(), SampleAvatarEntity.AssetSource.Zip);
           
            _targetAvatar.Hidden = true;

            foreach (var device in Microphone.devices) Debug.LogWarning(device);
#endif

#if UNITY_ANDROID
            foreach (var device in Microphone.devices) if (device == AndroidMicrophoneName) _currentMicrophoneName = device;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            foreach (var device in Microphone.devices) if (device == WindowsMicrophoneName) _currentMicrophoneName = device;
#endif
            if (String.IsNullOrEmpty(_currentMicrophoneName)) _currentMicrophoneName = Microphone.devices[0];
        }

        private IEnumerator StartCheckRecordingExisting()
        {            
            yield return new WaitUntil(() => _thisObjectData != null);
            yield return new WaitUntil(() => !String.IsNullOrEmpty(_thisObjectData.ThisTowerObjectId));
            
            CheckRecordAvailabilityAndSetButtonServerRpc(_thisObjectData.ThisTowerObjectId);
        }

        private void UserRequestToRecord()
        {
            InitNewDataServerRpc();
            if (_recordingCoroutineClient == null)
                _recordingCoroutineClient = StartCoroutine(RecordAndSendCoroutineClient());
        }
        private void UserRequestToStopRecRecording() => _currentEndTime = Time.time;
        private void UserRequestToPlay() => StartPlayingOnServerRpc(_thisObjectData.ThisTowerObjectId);
        private void UserRequestToStopPlaying() => StopPlayingOnServerRpc(_thisObjectData.ThisTowerObjectId);
        private void UserRequestToPausePlaying() => PausePlayingOnServerRpc(_thisObjectData.ThisTowerObjectId);

        private IEnumerator RecordAndSendCoroutineClient()
        {
            if (_isPlayingOnClient) yield break;

            if (!Microphone.IsRecording(_currentMicrophoneName))
                _recordedClip = Microphone.Start(_currentMicrophoneName, false, _recordLength, 22000);

            int lastAudioSample = 0;
            int frameCounter = 0;

            var startTime = Time.time;
            _currentEndTime =  startTime + _recordLength;

            while (_currentEndTime > Time.time)
            {
                if (Time.time - startTime > frameCounter * _recordPeriod)
                {
                    _view.SetTimerText((int)(Time.time - startTime) + 1, 0, true);

                    //motion
                    _frameToRecord = new AvatarFrame[OvrAvatarEntity.StreamLODCount];
                    for (int i = 0; i < OvrAvatarEntity.StreamLODCount; ++i)
                    {
                        var data = _sourceAvatar.RecordStreamData((StreamLOD)i);
                        _frameToRecord[i] = new AvatarFrame
                        {
                            Data = data
                        };

                        // Assume remote Avatar StreamLOD sizes are the same
                        float streamBytesPerSecond = _sourceAvatar.GetLastByteSizeForLodIndex(i) / StreamLodSnapshotIntervalSeconds[i];
                        AvatarLODManager.Instance.dynamicStreamLodBitsPerSecond[i] = (long)(streamBytesPerSecond * 8);
                    }

                    SendFrameServerRpc(_frameToRecord);
                    frameCounter++;

                    //sound
                    int audioPosition =
                        Microphone.GetPosition(
                            _currentMicrophoneName); //https://stackoverflow.com/questions/31171714/how-to-stream-live-audio-via-microphone-in-unity3d
                    int diff = audioPosition - lastAudioSample;

                    if (diff > 0)
                    {
                        float[] samples = new float[diff * _recordedClip.channels];
                        _recordedClip.GetData(samples, lastAudioSample);
                        SendSampleServerRpc(samples);
                    }

                    lastAudioSample = audioPosition;
                }
                yield return new WaitForEndOfFrame();
            }
            Microphone.End(_currentMicrophoneName);

            var currentLength = _currentEndTime - startTime;
            SendParamsServerRpc(
                _thisObjectData.ThisTowerObjectId,
                AvatarSessionData.AvatarAssetId.Value, 
                _recordedClip.name, 
                _recordedClip.channels, 
                _recordedClip.frequency, 
                false, 
                (int)(_currentEndTime - startTime), 
                _player.GetShift());

            _view.NotifyToWaitWhileSaving();
            _view.SetDurationAfterRecord((int)(_currentEndTime - startTime));
            //ui update after recorded saving complete is ClientRpc method below
        }

        private IEnumerator OpenAndPlayCoroutineServer(Guid objectId, bool downloadFromDataBase)
        {
            _isPlayingOnServer = true;
            _isPausedOnServer = false;

            _bufferToSend = 0;

            if (downloadFromDataBase) //for the first playing
            {
                var data = _saverServer.Open(objectId).AsTask();
                yield return new WaitUntil(() => data.IsCompleted);

                if (data == null)
                {
                    _playingCoroutineServer = null;
                    yield break;
                }

                _openedDataOnServer = data.Result;

                _recorderServer = new MovementRecorder();
                _recorderServer.Init();
                _recorderServer.SetRecord(_openedDataOnServer.Movement);
            }
            else _recorderServer.ClearCounter();
            
            RecordParams param = _openedDataOnServer.AvatarParameters;

            PrepareAvatarClientRpc(param.AvatarNumber, param.Shift);
            var audioClip = _openedDataOnServer.Audio;
            StartPlayClientRpc(audioClip.name, audioClip.frequency * param.LengthInSeconds, audioClip.channels, audioClip.frequency, false);

            int currentFrame = 0;
            int audioPosition = 0;
            var startTime = Time.time;
            int frameCounter = 0;            

            while (currentFrame < _openedDataOnServer.Movement.Count)
            {
                if (_isPausedOnServer) startTime += Time.deltaTime; //offset of the start time by the value of pause time
                if (!_isPausedOnServer && Time.time - startTime > frameCounter * _recordPeriod)
                {
                    //motion
                    _frameToPlay = _recorderServer.GetNextFrame();
                    if (_frameToPlay != null)
                    {
                        SendAvatarFrameMovementClientRpc(_frameToPlay);
                        currentFrame++;
                    }

                    //audio
                    float[] samples = new float[(int)(_recordPeriod * audioClip.frequency)];

                    if (samples.Length + audioPosition <= audioClip.samples)
                    {
                        audioClip.GetData(samples, audioPosition);
                        SendAudioSampleClientRpc(samples);
                        audioPosition += (int)(_recordPeriod * audioClip.frequency);
                    }
                    frameCounter++;
                }
                yield return new WaitForEndOfFrame();
            }
            _isPlayingOnServer = false;
            _isPausedOnServer = false;
            _playingCoroutineServer = null;
        }
        
        //save on server
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void InitNewDataServerRpc()
        {
            _serverFramesToSave = new List<AvatarFrame[]>();
            _serverAudioFloatToSave = new List<float>();
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SendFrameServerRpc(AvatarFrame[] frame)
        {
            _serverFramesToSave.Add(frame);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SendSampleServerRpc(float[] samples)
        {
            _serverAudioFloatToSave.AddRange(samples);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SendParamsServerRpc(string objectId, int avatarNumber, string audioName, int channels, int frequency, bool stream, int lengthSeconds, Vector3 shift)
        {
            AudioClip audio = AudioClip.Create(audioName, _serverAudioFloatToSave.Count, channels, frequency, stream);
            audio.SetData(_serverAudioFloatToSave.ToArray(), 0);

            RecordParams param = new RecordParams(avatarNumber, lengthSeconds, shift);
            _serverDataToSave = new AvatarRecorderData(_serverFramesToSave, audio, param);

            SaveAsync(_serverDataToSave, new Guid(objectId), lengthSeconds).Forget();
        }
        private async UniTask SaveAsync(AvatarRecorderData data, Guid objectId, int duration)
        {
            await _saverServer.Save(_serverDataToSave, objectId, duration);
            IdleStateAfterRecordSavedClientRpc();
        }


        //play on client
        /// <param name="objectId">recorder object id</param>
        /// <param name="downloadFromDataBase">true when first play, false when cycle playing</param>
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void StartPlayingOnServerRpc(string objectId, bool downloadFromDataBase = true)
        {
            if (_playingCoroutineServer == null && !_isPlayingOnServer) _playingCoroutineServer = StartCoroutine(OpenAndPlayCoroutineServer(new Guid(objectId), downloadFromDataBase));
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void StopPlayingOnServerRpc(string objectId)
        {
            if (_playingCoroutineServer != null)
            {
                StopCoroutine(_playingCoroutineServer);
                _isPlayingOnServer = false;
                _playingCoroutineServer = null;
            }
            StopPlayClientRpc();
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void PausePlayingOnServerRpc(string objectId)
        {
            _isPausedOnServer = !_isPausedOnServer;
            PausePlayClientRpc(_isPausedOnServer);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void CheckRecordAvailabilityAndSetButtonServerRpc(string objectId)
        {
            CheckButtonAsyncServer(objectId).Forget();
        } 
        private async UniTask CheckButtonAsyncServer(string objectId)
        {
            var result = await _saverServer.CheckRecordAvailability(new Guid(objectId));            

            SetPlayButtonClientRpc(result, _isPlayingOnServer);
        }        


        [ClientRpc]
        private void SetPlayButtonClientRpc(int recordLength, bool isPlayingOnServer, ClientRpcParams clientRpcParams = default)
        {
            if (recordLength != -1) _view.RecordExists(recordLength);
            if (isPlayingOnServer) _view.BlockPhysButton();
        }

        [ClientRpc]
        private void IdleStateAfterRecordSavedClientRpc()
        {
            _view.SetButtons(true, false, true);
            _view.SetIdleConeFromRec();
            _view.SetTimerText(0);

            _recordingCoroutineClient = null;
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void PrepareAvatarClientRpc(int avatarNum, Vector3 shift, ClientRpcParams clientRpcParams = default)
        {
            ///it is necessary for the case when playback on one of the clients did not have time to complete
            if (_tickPlaySoundCoroutineClient != null) StopCoroutine(_tickPlaySoundCoroutineClient);
            _audioOutput.Stop();
            _isBlockedWhilePauseClient = false;
            ///

            _isPlayingOnClient = true;

            _view.SetButtons(false, false, false);
            _view.BlockPhysButton();

            _targetAvatar.ReloadAvatarManually(avatarNum.ToString(), SampleAvatarEntity.AssetSource.StreamingAssets);

            _shiftObject.localPosition = -shift + Vector3.up * 1.4f;

            _movementToPlay = new Queue<AvatarFrame[]>();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void StartPlayClientRpc(string audioName, int length, int channels, int frequency, bool stream, ClientRpcParams clientRpcParams = default)
        {
            _audioBufferToPlay = 0;
            _playingPosition = 0;
            _playingClip = AudioClip.Create(audioName, length, channels, frequency, stream);

            _view.ClickPlayExternal();
            _tickPlaySoundCoroutineClient = StartCoroutine(PlayingCoroutineClient());
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void StopPlayClientRpc()
        {
            if (_tickPlaySoundCoroutineClient != null) StopCoroutine(_tickPlaySoundCoroutineClient);
            _audioOutput.Stop();
            _isBlockedWhilePauseClient = false;

            _targetAvatar.Hidden = true;
            _isPlayingOnClient = false;
            _movementToPlay.Clear();
            
            _view.ClickStopPlayExternal();
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void PausePlayClientRpc(bool setPause)
        {
            _isBlockedWhilePauseClient = setPause;
            _view.BlockPhysButton(); //when user connects while pause to prevent start button working on !owner

        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SendAvatarFrameMovementClientRpc(AvatarFrame[] frame)
        {
            if (_isPlayingOnClient)
            {
                _movementToPlay.Enqueue(frame);
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SendAudioSampleClientRpc(float[] samples)
        {
            if (!_isPlayingOnClient) return;
            if (_playingPosition + samples.Length > _playingClip.samples) return;

            _playingClip.SetData(samples, _playingPosition);
            _playingPosition += samples.Length;
            _audioBufferToPlay++;
        }

        private IEnumerator PlayingCoroutineClient()
        {
            _view.NotifyToWaitWhileDownloading();

            while (_audioBufferToPlay < DelayBuffer)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitUntil(() => _movementToPlay.Count > DelayBuffer);

            _targetAvatar.Hidden = false;

            _audioOutput.clip = _playingClip;
            int clipLenghtForUi = (int)_audioOutput.clip.length;
            _audioOutput.Play();
            var startTime = Time.time;
            int frameCounter = 0;
            _isBlockedWhilePauseClient = false;

            while (_movementToPlay.Count > 0)
            {
                if (_isBlockedWhilePauseClient) startTime += Time.deltaTime; //offset of the start time by the value of pause time
                if (_isBlockedWhilePauseClient) _audioOutput.Pause();
                else if (!_audioOutput.isPlaying) _audioOutput.Play();

                if (!_isBlockedWhilePauseClient && Time.time - startTime > frameCounter * _recordPeriod)
                {
                    _view.SetTimerText((int)(Time.time - startTime) + 1, clipLenghtForUi);

                    var data = _movementToPlay.Dequeue();
                    for (int i = 0; i < OvrAvatarEntity.StreamLODCount; ++i)
                    {
                        if (_targetAvatar != null)
                            _targetAvatar.ApplyStreamData(data[i].Data);
                    }
                    frameCounter++;
                }
                if (!_isBlockedWhilePauseClient && !_audioOutput.isPlaying) break;
                yield return new WaitForEndOfFrame();
            }
            _targetAvatar.Hidden = true;
            _audioOutput.Stop();
            _isPlayingOnClient = false;
            _isBlockedWhilePauseClient = false;
            StartPlayingOnServerRpc(_thisObjectData.ThisTowerObjectId, false);

            _tickPlaySoundCoroutineClient = null;
        }
    }
}
