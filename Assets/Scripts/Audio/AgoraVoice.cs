using System;
using System.Collections;
using agora_gaming_rtc;
using Assets.Scripts.API;
using Assets.Scripts.Audio;
using Data;
using TMPro;
using UnityEngine;
using Zenject;

public class AgoraVoice : MonoBehaviour
{
    private const int VoiceAudioChannels = 1;
    private const int VoiceSampleRate = 48000;

    #region Inspector

    [SerializeField] private string AppID = "85103af005b34b4b94656eeae57af085";
    public string channelName = "hall";
    [SerializeField] private DiscoveryServiceStaticData _discoveryServiceData;
    [SerializeField] private string token = "e3b680115e3d475fbc7da0287e000af0";

    #endregion

    #region PrivateVariables

    private Coroutine _coroutine;
    private IRtcEngine _mRtcEngine = null;

    private AndroidJavaObject _audioManager;

    private AndroidJavaObject _deviceAudio
    {
        get
        {
            if (_audioManager == null)
            {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaClass audioManagerClass = new AndroidJavaClass("android.media.AudioManager");
                AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");

                _deviceStreamVolume = audioManagerClass.GetStatic<int>("STREAM_MUSIC");
                string Context_AUDIO_SERVICE = contextClass.GetStatic<string>("AUDIO_SERVICE");

                _audioManager = context.Call<AndroidJavaObject>("getSystemService", Context_AUDIO_SERVICE);

                if (_audioManager != null)
                    Debug.Log("[AndroidNativeVolumeService] Android Audio Manager successfully set up");
                else
                    Debug.Log("[AndroidNativeVolumeService] Could not read Audio Manager");
            }

            return _audioManager;
        }
    }

    private IApiService _apiService;

    private int _currentAudioChannelID = 0;
    private string _currentAudioChannel = string.Empty;
    private string _currentAudioChannelShortName = string.Empty;
    private string _defaultAudioChannelFullName = string.Empty;
    private string _lastConnectingAudioChannelShortName = string.Empty;
    private int _deviceStreamVolume;
    private float _currentVolume = 0;
    private int _maxVolume = 0;

    private IAudioEffectManager _audioEffectManager;
    private SpatialAudio _spatialAudio;

    public bool IsBusy { get; private set; }
    public string CurrentAudioChannelShortName => _currentAudioChannelShortName;
    public string CurrentAudioChannelFullName => _currentAudioChannel;
    public string DefaultAudioChannelFullName => _defaultAudioChannelFullName;

    public event Action<AudioFrame> AudioFrameRecorded;

    #endregion

    #region Events

    public event Action<uint> LocalPlayerJoinChannel;
    public event Action<uint> RemotePlayerJoinChannel;
    public event Action<uint> PlayerLeaveChannel;
    public event Action<SpatialAudio> LocalPlayerSpatialSoundSetted;

    #endregion

    #region UnityMethods

    private void Awake()
    {
#if !UNITY_SERVER
        _defaultAudioChannelFullName = GetFullChannelName(channelName);
#endif
    }

    private void Start()
    {
#if !UNITY_SERVER
        _mRtcEngine = IRtcEngine.GetEngine(AppID);

        _audioEffectManager = _mRtcEngine.GetAudioEffectManager();
        _mRtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
        //_mRtcEngine.EnableSpatialAudio(true);
        //_mRtcEngine.SetRemoteUserSpatialAudioParams()
        _mRtcEngine.EnableSoundPositionIndication(true);

        _mRtcEngine.OnUserJoined += OnUserJoined;
        _mRtcEngine.OnError += OnError;

        _mRtcEngine.OnUserOffline += OnUserOffline;
        _mRtcEngine.GetAudioRawDataManager().SetOnRecordAudioFrameCallback(OnFrameRecorded);
        _mRtcEngine.GetAudioRawDataManager().RegisterAudioRawDataObserver();
#endif
    }

    private void OnFrameRecorded(AudioFrame audioFrame)
    {
        AudioFrameRecorded?.Invoke(audioFrame);
    }

    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(ReferenceEquals(_mRtcEngine,null)) return;
        
        _mRtcEngine.AdjustPlaybackSignalVolume((int)(100 * GetSystemVolume()));
#endif
    }

    void OnDestroy()
    {
        if (_mRtcEngine != null)
        {
            _mRtcEngine.OnError -= OnError;
            _mRtcEngine.OnJoinChannelSuccess -= OnJoinChannelSuccess;
            _mRtcEngine.OnUserOffline -= OnUserOffline;
            _mRtcEngine.OnUserJoined -= OnUserJoined;
            _mRtcEngine.GetAudioRawDataManager().UnRegisterAudioRawDataObserver();
            IRtcEngine.Destroy();
        }
    }

    #endregion

    #region PublicMethods

    [Inject]
    public void Init(IApiService apiService)
    {
        _apiService = apiService;
    }

    public void JoinDefaultChannel()
    {
        JoinChannel(channelName);
    }

    public void JoinChannel(string audioChannelName)
    {
        if (IsBusy)
        {
            return;
        }

        bool isUserAuth = _apiService.CheckUserAuthentication();
        if (!isUserAuth)
        {
            Debug.LogError("JoinChannel failed: User is not authorized");
            return;
        }

        var zoneName = string.IsNullOrEmpty(audioChannelName) ? channelName : audioChannelName;
        string resultName = GetFullChannelName(zoneName);

        _lastConnectingAudioChannelShortName = zoneName;

        _coroutine = StartCoroutine(JoinChannelCoroutine(resultName));
    }

    public void LeaveChannel()
    {
        _mRtcEngine.LeaveChannel();
    }

    public IAudioEffectManager GetAudioEffectManager()
    {
        return _audioEffectManager;
    }

    public void SaveCurrentPlayerSpatialAudio(SpatialAudio spatialAudio)
    {
        _spatialAudio = spatialAudio;
        LocalPlayerSpatialSoundSetted?.Invoke(_spatialAudio);
    }

    public SpatialAudio GetCurrentPlayerSpatialAudio()
    {
        return _spatialAudio;
    }

    #endregion

    #region PrivateMethods

    private int GetDeviceMaxVolume()
    {
        _maxVolume = _deviceAudio.Call<int>("getStreamMaxVolume", _deviceStreamVolume);
        return _maxVolume;
    }

    private float GetSystemVolume()
    {
        _currentVolume = _deviceAudio.Call<int>("getStreamVolume", _deviceStreamVolume);
        float scaledVolume = (float) _currentVolume / (float) GetDeviceMaxVolume();

        return scaledVolume;
    }

    private void SetDeviceVolume(float volumeValue)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetSystemVolume();
        int scaledVolume = (int)(volumeValue * (float)_maxVolume);
        
        _deviceAudio.Call("setStreamVolume", _deviceStreamVolume, scaledVolume, 1);
#endif
    }

    private string GetFullChannelName(string shortChannelName)
    {
        string suffixVersion = "prod";
        var appVersion = _discoveryServiceData.Assembly.AssemblyType;

        switch (appVersion)
        {
            case Enumenators.Server.Profile.Develop:
                suffixVersion = "dev";
                break;
            case Enumenators.Server.Profile.Demo:
                suffixVersion = "demo";
                break;
            case Enumenators.Server.Profile.Production:
                suffixVersion = "prod";
                break;
            case Enumenators.Server.Profile.Test:
                suffixVersion = "test";
                break;
            default:
                break;
        }

        string suffixId = PlayerPrefs.GetString("CurrentSceneTransitionId");

        return $"{shortChannelName}_{suffixVersion}_{suffixId}";
    }

    #endregion

    #region Callbacks

    private void OnJoinChannelSuccess(string channelname, uint uid, int elapsed)
    {
        _currentAudioChannel = channelname;
        _currentAudioChannelShortName = _lastConnectingAudioChannelShortName;
        LocalPlayerJoinChannel?.Invoke(uid);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        RemotePlayerJoinChannel?.Invoke(uid);
    }

    private void OnError(int error, string msg)
    {
        string description = IRtcEngine.GetErrorDescription(error);
        Debug.LogError($"Agora error {error} : {msg} {description}");
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            IsBusy = false;
        }

        if (error == 1017)
        {
            Debug.LogWarning($"Agora reconnecting to channel...");
            _coroutine = StartCoroutine(JoinChannelCoroutine(_currentAudioChannel));
        }
    }

    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        PlayerLeaveChannel?.Invoke(uid);
    }

    #endregion

    #region Coroutines

    private IEnumerator JoinChannelCoroutine(string audioChannelName)
    {
        if (string.IsNullOrEmpty(audioChannelName))
        {
            _coroutine = null;
            yield break;
        }

        IsBusy = true;

        _currentAudioChannel = string.Empty;
        _currentAudioChannelShortName = string.Empty;

        while (!_currentAudioChannel.Equals(audioChannelName))
        {
            if (_mRtcEngine != null)
            {
                if (_currentAudioChannelID != 0)
                    _mRtcEngine.LeaveChannel();

                _currentAudioChannelID = _mRtcEngine.JoinChannel(audioChannelName, "extra", 0);
            }

            yield return new WaitForSeconds(1f);
        }

        _coroutine = null;
        IsBusy = false;
    }

    #endregion
}