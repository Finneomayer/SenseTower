using System;
using Assets.Mechanics.Network.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Vuplex.WebView;
using Unity.VisualScripting;
using Assets.Scripts.Space;

public class CinemaEnviromentControl : NetworkBehaviour
{
    #region Inspector
    [SerializeField] private BrowserAdminControlPanel _browserAdminControlPanel;
    [SerializeField] private CustomSlider _customSlider;
    [SerializeField] private CanvasWebViewPrefab _browserPrefab;
    [SerializeField] private AdminPlace _adminPlace;
    [SerializeField] private Color fadeColor;
    [SerializeField] private List<MeshRenderer> fadeObjects = new();
    [SerializeField] private List<MeshRenderer> emissionObjects = new();
    [SerializeField] private List<GameObject> _lightObjects = new List<GameObject>();
    #endregion

    [SerializeField] Dictionary<string, Color> startColor = new();
    private float _volumeValue = 1;
    private string _currentVolumeOnClient = string.Empty;
    private bool _isSceneDarkened = false;
    private bool _browserInitialized;
    private float _sliderPreviousValue;
    private event Action<float> SliderValueChanged;
    public void Start()
    {
#if !UNITY_SERVER
        //_customSlider.PointerExit += OnPointerExit;
        //_customSlider.onValueChanged.AddListener(OnSliderChangeValue);
        SliderValueChanged += OnSliderChangeValue; //this is a workaround because the standard value works not correctly
        _browserAdminControlPanel.ToogleLight += OnToogleLight;
        _adminPlace.AdminChange += OnAdminChange;
        for (int i = 0; i < fadeObjects.Count; i++)
        {
            var tempObj = fadeObjects[i];
            if (tempObj.sharedMaterial.HasProperty("_MainColor"))
            {
                if (tempObj.sharedMaterials.Length > 0)
                {
                    for (int y = 0; y < tempObj.sharedMaterials.Length; y++)
                    {
                        startColor.Add(tempObj.gameObject.name + y, tempObj.sharedMaterials[y].GetColor("_MainColor"));
                    }
                }
                else
                    startColor.Add(tempObj.gameObject.name, tempObj.sharedMaterial.GetColor("_MainColor"));
            }
        }
#endif

#if UNITY_SERVER
        NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
#endif
    }

    public void SetSliderDefaultValue()
    {
        _customSlider.value = 1;
    }

    private void FixedUpdate()
    {
        //this is a workaround because the standard value works not correctly
        //you can check it: Debug.LogWarning($"native: {_customSlider.value}  -- workaround {_customSlider.GetValue()}");
        var newValue = _customSlider.GetValue();
        if (Math.Abs(_sliderPreviousValue - newValue) > 0.001f) SliderValueChanged?.Invoke(newValue);
        _sliderPreviousValue = newValue;
    }

    private void OnDisable()
    {
#if !UNITY_SERVER
        //_customSlider.PointerExit -= OnPointerExit;
        //_customSlider.onValueChanged.RemoveAllListeners();
        SliderValueChanged -= OnSliderChangeValue;//this is a workaround because the standard value works not correctly
        _adminPlace.AdminChange -= OnAdminChange;
        _browserAdminControlPanel.ToogleLight -= OnToogleLight;

        _browserPrefab.WebView.UrlChanged -= OnUrlChanged;
#endif
    }

    public async override void OnNetworkSpawn()
    {
        await _browserPrefab.WaitUntilInitialized();

        _browserPrefab.WebView.UrlChanged += OnUrlChanged;
        GetSceneLightServerRpc(NetworkManager.Singleton.LocalClientId);
        //GetSceneVolumeServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public override void OnNetworkDespawn()
    {
#if UNITY_SERVER
        NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
#endif
    }
    private void Update()
    {
        //if (IsServer) return;
        //if (_browserPrefab.WebView != null && _browserPrefab.WebView.IsInitialized)
        //{
        //    _browserPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {_currentVolumeOnClient})");
        //}
    }

    private void OnAdminChange(ulong previousValue, ulong newValue)
    {
        if (!_adminPlace.IsAdminSet())
            OnClearAdmin();
    }

    private void ChangeSceneLight(bool isSceneDarkened) 
    {
        if (isSceneDarkened)
        {
            for (int i = 0; i < fadeObjects.Count; i++)
            {
                var tempObj = fadeObjects[i];
                if (tempObj.sharedMaterial.HasProperty("_MainColor"))
                {
                    if (tempObj.sharedMaterials.Length > 0)
                    {
                        for (int y = 0; y < tempObj.sharedMaterials.Length; y++)
                        {
                            tempObj.sharedMaterials[y].SetColor("_MainColor", fadeColor);
                        }
                    }
                        
                    else
                        tempObj.sharedMaterial.SetColor("_MainColor", fadeColor);
                }
            }
            for (int i = 0; i < emissionObjects.Count; i++)
            {
                emissionObjects[i].sharedMaterial.DisableKeyword("_EMISSION");
            }
        }
        else 
        {
            for (int i = 0; i < fadeObjects.Count; i++)
            {
                var tempObj = fadeObjects[i];
                if (tempObj.sharedMaterials.Length > 0)
                {
                    for (int y = 0; y < tempObj.sharedMaterials.Length; y++)
                    {
                        if (startColor.ContainsKey(tempObj.gameObject.name + y))
                        {
                            tempObj.sharedMaterials[y].SetColor("_MainColor", startColor[tempObj.gameObject.name + y]);
                        }
                    }
                }
                else
                {
                    if (startColor.ContainsKey(tempObj.gameObject.name))
                    {
                        tempObj.sharedMaterial.SetColor("_MainColor", startColor[tempObj.gameObject.name]);
                    }
                }
            }
            for (int i = 0; i < emissionObjects.Count; i++)
            {
                emissionObjects[i].sharedMaterial.EnableKeyword("_EMISSION");
            }
        }

        _lightObjects.ForEach(x => x.SetActive(isSceneDarkened));

        //int culingMask = isSceneDarkened ? LayerMask.GetMask("Grabable", "Presentation","Body","Default","UI") : -1;
        //Camera targetCamera = Camera.main;
        //
        //targetCamera.clearFlags = isSceneDarkened ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        //targetCamera.cullingMask = culingMask;
        //targetCamera.backgroundColor = fadeColor;
    }

    private void OnToogleLight(bool lightValue)
    {
        ChangeSceneLight(lightValue);
        ToogleLightServerRpc(NetworkManager.Singleton.LocalClientId, lightValue);
    }

    private void OnSliderChangeValue(float volumeValue)
    {
        string volumeValueInString = volumeValue.ToString().Replace(',','.');
        _currentVolumeOnClient = volumeValueInString;
        Debug.LogWarning(_currentVolumeOnClient);

        if (IsServer) return;
        if (_browserPrefab.WebView != null && _browserPrefab.WebView.IsInitialized)
        {
            _browserPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {_currentVolumeOnClient})");
        }
    }

    //private void OnPointerExit(float volumeValue)
    //{
    //    ChangeVolumeServerRpc(NetworkManager.Singleton.LocalClientId, volumeValue);
    //}

    private void OnClearAdmin() 
    {
        if (_browserPrefab.WebView!= null)
            _browserPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = 1)");

        //ChangeVolumeServerRpc(NetworkManager.Singleton.LocalClientId, 1);

        ChangeSceneLight(false);
        ToogleLightServerRpc(NetworkManager.Singleton.LocalClientId, false);
    }

    #region Server
    //[ServerRpc(RequireOwnership = false)]
    //private void GetSceneVolumeServerRpc(ulong mediatorId) 
    //{
    //    ChangeVolumeCurrentClientRpc(mediatorId, _volumeValue);
    //}

    [ServerRpc(RequireOwnership = false)]
    private void GetSceneLightServerRpc(ulong mediatorId)
    {
        ToogleLightCurrentClientRpc(mediatorId, _isSceneDarkened);
    }

    //[ServerRpc(RequireOwnership = false)]
    //private void ChangeVolumeServerRpc(ulong adminId, float volumeValue) 
    //{
    //    _volumeValue = volumeValue;
    //    ChangeVolumeOtherClientRpc(adminId, volumeValue);
    //}

    [ServerRpc(RequireOwnership = false)]
    private void ToogleLightServerRpc(ulong mediatorId, bool isSceneDarkened) 
    {
        _isSceneDarkened = isSceneDarkened;
        ToogleLightOtherClientRpc(mediatorId,isSceneDarkened);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (_adminPlace.IsUserAdmin(clientId))
        {
            _volumeValue = 1;
            _isSceneDarkened = false;

            //ChangeVolumeOtherClientRpc(clientId, _volumeValue);
            ToogleLightServerRpc(clientId, _isSceneDarkened);
        }
    }

    #endregion

    #region Client

    //[ClientRpc]
    //private void ChangeVolumeOtherClientRpc(ulong adminId, float volumeValue) 
    //{
    //    if (NetworkManager.Singleton.LocalClientId != adminId)
    //    {
    //        string volumeValueInString = volumeValue.ToString().Replace(',', '.');
    //        _browserPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {volumeValueInString})");

    //        _currentVolumeOnClient = volumeValueInString;
    //    }
    //}

    [ClientRpc]
    private void ToogleLightOtherClientRpc(ulong mediatorId,bool isSceneDarkened)
    {
        if(NetworkManager.Singleton.LocalClientId != mediatorId)
            ChangeSceneLight(isSceneDarkened);
    }

    //[ClientRpc]
    //private void ChangeVolumeCurrentClientRpc(ulong adminId, float volumeValue)
    //{
    //    if (NetworkManager.Singleton.LocalClientId == adminId)
    //    {
    //        string volumeValueInString = volumeValue.ToString().Replace(',', '.');
    //        if (_browserPrefab != null)
    //        {
    //            SetVolume(volumeValueInString);
    //        }
    //    }
    //}

    //private void SetVolume(string volumeValue) 
    //{
    //    _currentVolumeOnClient = volumeValue;
    //    _browserPrefab.WebView.ExecuteJavaScript($"document.querySelectorAll('video, audio').forEach(mediaElement => mediaElement.volume = {volumeValue})");
    //}

    private void OnBrowserInitialized(object sender, System.EventArgs e)
    {
        GetSceneLightServerRpc(NetworkManager.Singleton.LocalClientId);
        //GetSceneVolumeServerRpc(NetworkManager.Singleton.LocalClientId);
        
        _browserPrefab.WebView.UrlChanged += OnUrlChanged;
    }

    private void OnUrlChanged(object sender, UrlChangedEventArgs e)
    {
        //GetSceneVolumeServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ClientRpc]
    private void ToogleLightCurrentClientRpc(ulong mediatorId, bool isSceneDarkened)
    {
        if (NetworkManager.Singleton.LocalClientId == mediatorId)
            ChangeSceneLight(isSceneDarkened);
    }
    #endregion
}
