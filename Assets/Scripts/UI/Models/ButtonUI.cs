using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    public Button InteractElement;
    public Image Background;
    public Image PreviewImage;
    public TMP_Text Text;
    public Sprite OnClickImage;

    public bool IsHighlighting;
    public bool IsToggle;

    public bool IsInteractable { get; private set; } = true;

    private Sprite _startSprite;
    private bool _isCLicked = false;

    private void OnEnable()
    {
        Subscribe();
    }

    private async void Subscribe()
    {
        await Task.Delay(1000);
        InteractElement.onClick.AddListener(OnClickInteractElement);
    }

    private void Start()
    {
        if (OnClickImage == null) return;

        _startSprite = PreviewImage.sprite;
    }

    private void OnDisable()
    {
        InteractElement.onClick.RemoveListener(OnClickInteractElement);
    }

    public void SetStartStateButton() 
    {
        if (OnClickImage != null && _startSprite != null)
            PreviewImage.sprite = _startSprite;

        _isCLicked = false;
    }

    public void SetButtonActive(bool active)
    {
        ChangeBackgroundAlpha(active);
    }

    public void SetButtonInteractable(bool interactable)
    {
        IsInteractable = interactable;
        InteractElement.interactable = interactable;
        ChangeTextAlpha(interactable);
        ChangeImageAlpha(interactable);
    }

    private void ChangeBackgroundAlpha(bool isVisible)
    {
        if (Background == null)
        {
            return;
        }
        var tempColor = Background.color;
        tempColor.a = isVisible ? 1f : 0f;
        Background.color = tempColor;
    }

    private void ChangeTextAlpha(bool interactable)
    {
        if (Text == null)
        {
            return;
        }
        var tempColor = Text.color;
        tempColor.a = interactable ? 1f : 0.1f;
        Text.color = tempColor;
    }

    private void ChangeImageAlpha(bool interactable)
    {
        if (PreviewImage == null)
        {
            return;
        }
        var tempColor = PreviewImage.color;
        tempColor.a = interactable ? 1f : 0.1f;
        PreviewImage.color = tempColor;
    }

    private void OnClickInteractElement() 
    {
        ToggleSprite();
        SetClicked().Forget();
    }

    private void ToggleSprite()
    {
        if (OnClickImage == null)
        {
            return;
        }

        _isCLicked = !_isCLicked;

        PreviewImage.sprite = _isCLicked ? OnClickImage : _startSprite;
    }

    private async UniTask SetClicked()
    {
        if (!IsHighlighting)
        {
            return;
        }
        if (IsToggle)
        {
            return;
        }

        SetButtonActive(true);
        await UniTask.Delay(100);
        SetButtonActive(false);
    }
}
