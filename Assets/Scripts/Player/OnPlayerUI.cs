using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using Unity.XR.CoreUtils;
using static OVRPlugin;
using Cysharp.Threading.Tasks;

public class OnPlayerUI : MonoBehaviour
{
    [SerializeField] private Canvas _messageCanvas;
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _blackScreen;
    [Space]
    [SerializeField] private Button _exitButton;
    [SerializeField] private Toggle _inRoomToggle;
    [SerializeField] private Toggle _continuousMovingToggle;
    [SerializeField] private Toggle _vignetteToggle;
    [Space]
    public Image BlackMask; //controls by SceneChangerView
    public Image LoadingAnimation; //controls by SceneChangerView
    [SerializeField] private TMP_Text _loadingText;

    public event Action LogoutPressed;
    public event Action<bool> ToggleInRoomChanged;
    public event Action<bool> ToggleContinuousMovingChanged;
    public event Action<bool> ToggleVignetteChanged;

    private TMP_Text _messageText;
    private Coroutine _shomMessageCoroutine;
    private bool _isShowingMessage = false;
    private bool _isShowingMenu = false;
    private Coroutine _fadeCoroutine = null;

    private Camera _playerCamera;
    private int _defaultCameraMask;
    private Canvas _customCanvas;

    private void Start()
    {
        _messageText = GetComponentInChildren<TMP_Text>();

        _exitButton.onClick.AddListener(() => LogoutPressed?.Invoke());
        _inRoomToggle.onValueChanged.AddListener((e) => ToggleInRoomChanged?.Invoke(_inRoomToggle.isOn));
        _continuousMovingToggle.onValueChanged.AddListener((e) => ToggleContinuousMovingChanged?.Invoke(_continuousMovingToggle.isOn));
        _vignetteToggle.onValueChanged.AddListener((e) => ToggleVignetteChanged?.Invoke(_vignetteToggle.isOn));
    }
    public async UniTask FadeToBlackDefault(string message = "")
    {
        _customCanvas = null;
        await FadeToBlack(1, message, showAnimation: true);
    }

    public async UniTask FadeToBlackCustomCanvas(Canvas canvas)
    {
        _customCanvas = canvas;
        _customCanvas.renderMode = RenderMode.WorldSpace;
        _customCanvas.worldCamera = _playerCamera;
        _customCanvas.sortingLayerName = "Layer 1";
        _customCanvas.sortingOrder = 2;

        await FadeToBlack(1, "", showAnimation: false);
    }

    private async UniTask FadeToBlack(float duration, string message, bool showAnimation)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(fadeToBlack: true, duration, message, showAnimation));
        await UniTask.WaitUntil(() => _fadeCoroutine == null);
    }

    public async UniTask FadeToTransparent(float duration = 1)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeCoroutine(fadeToBlack: false, duration = 1));

        await UniTask.WaitUntil(() => _fadeCoroutine == null);
    }
    
    private IEnumerator FadeCoroutine(bool fadeToBlack, float duration, string message = "", bool showAnimation = true)
    {
        _playerCamera.cullingMask = _defaultCameraMask;
        BlackMask.gameObject.SetActive(true);

        if (fadeToBlack)
        {
            BlackMask.color = new Color(BlackMask.color.r, BlackMask.color.g, BlackMask.color.b, 0);
        }
        else
        {
            BlackMask.color = new Color(BlackMask.color.r, BlackMask.color.g, BlackMask.color.b, 1);
            LoadingAnimation.enabled = false;
            _loadingText.enabled = false;
            if (_customCanvas != null) _customCanvas.enabled = false;
        }
        
        Color targetColor = BlackMask.color;
        targetColor.a = fadeToBlack ? 1 : 0;

        float timeLeft = duration;

        _blackScreen.gameObject.SetActive(true);
        while (timeLeft > 0)
        {
            yield return null;
            BlackMask.color = Color.Lerp(BlackMask.color, targetColor, Time.deltaTime / timeLeft);
            timeLeft -= Time.deltaTime;
        }
        BlackMask.color = targetColor;

        if (fadeToBlack)
        {
            if (showAnimation) LoadingAnimation.enabled = true;
            _loadingText.enabled = true;
            _loadingText.text = message;
            _playerCamera.cullingMask = (1 << LayerMask.NameToLayer("VisibleWhileBlackScreen"));
            if (_customCanvas != null) _customCanvas.enabled = true;
        }
        else
        {
            //BlackMask.gameObject.SetActive(false);
            _playerCamera.cullingMask = _defaultCameraMask;
            BlackMask.gameObject.SetActive(false);
        }

        _fadeCoroutine = null;
    }


    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.position = parent.transform.position;
    }

    private void OnDestroy()
    {
        _exitButton.onClick.RemoveAllListeners();
        _inRoomToggle.onValueChanged.RemoveAllListeners();
    }

    private void MenuExecute(InputAction.CallbackContext context)
    {
        if (!_isShowingMenu)
        {
            _menuCanvas.enabled = true;
            _isShowingMenu = true;
        }
        else
        {
            _menuCanvas.enabled = false;
            _isShowingMenu = false;
        }
    }

    public void ShowMessage(string message, float time)
    {
        if (!_isShowingMessage)
        {
            _shomMessageCoroutine = StartCoroutine(ShowMessageInTime(time));
            _messageText.text = message;
        }
    }

    private IEnumerator ShowMessageInTime(float time)
    {
        _isShowingMessage = true;
        _messageCanvas.enabled = true;
        yield return new WaitForSeconds(time);
        _isShowingMessage = false;
        _messageCanvas.enabled = false;
    }

    public void BlockButtons()
    {
        _exitButton.interactable = false;
        _inRoomToggle.interactable = false;
        _continuousMovingToggle.interactable = false;
        _vignetteToggle.interactable = false;
    }

    public void UnblockButtons()
    {
        _exitButton.interactable = true;
        _inRoomToggle.interactable = true;
        _continuousMovingToggle.interactable = true;
        _vignetteToggle.interactable = true;
    }

    public void SetPlayerCamera(Camera cam)
    {
        _playerCamera = cam;
        _defaultCameraMask = _playerCamera.cullingMask;

        _messageCanvas.renderMode = RenderMode.WorldSpace;
        _messageCanvas.worldCamera = cam;

        _menuCanvas.renderMode = RenderMode.WorldSpace;
        _menuCanvas.worldCamera = cam;

        _blackScreen.renderMode = RenderMode.ScreenSpaceCamera;
        _blackScreen.worldCamera = cam;
    }
}
