using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIVisibility : MonoBehaviour
{
    #region Inspector

    [SerializeField] private CanvasGroup _canvasGroup;

    public Camera PlayerCamera;

    [Range(0, 1)] public float DeltaOffset;

    #endregion

    private bool _isVisible = true;

    private void Start()
    {
        if (PlayerCamera == null)
            PlayerCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float delta = Vector3.Distance(transform.position, PlayerCamera.transform.position);
        float alpha = Mathf.Clamp(delta, 0, 1);
        if (alpha < 0.5f)
        {
            alpha = 0;
        }
        else
        {
            alpha += DeltaOffset;
        }

        SetVisibility(1 - alpha);
    }

    public void Show()
    {
        _isVisible = true;
    }

    public void Hide()
    {
        _isVisible = false;
        SetVisibility(0);
    }

    private void SetVisibility(float alpha)
    {
        if (!_isVisible)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            return;
        }

        float resultAlpha = alpha;
        _canvasGroup.alpha = resultAlpha;
        if (resultAlpha > 0.8f)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        else
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }
}