using System;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private GameObject ModalPanel;
    [SerializeField] private TMP_Text Message;
    [SerializeField] private ButtonUI ConfirmButton;
    [SerializeField] private ButtonUI CancelButton;

    private bool _isBusy;
    private LookAtPlayer _parentRotator;

    public bool? ModalResult { get; private set; }

    public void SetRotator(LookAtPlayer rotator)
    {
        _parentRotator = rotator;
    }

    private void Awake()
    {
        if (!_isBusy)
        {
            ModalPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        AddButtonListener(ConfirmButton, OnConfirmButtonClick);
        AddButtonListener(CancelButton, OnCancelButtonClick);
    }

    private void OnDisable()
    {
        RemoveButtonListener(ConfirmButton, OnConfirmButtonClick);
        RemoveButtonListener(CancelButton, OnCancelButtonClick);
    }

    private void AddButtonListener(ButtonUI button, UnityAction listener)
    {
        if (button == null || button.InteractElement == null)
        {
            return;
        }

        button.InteractElement.onClick.AddListener(listener);
    }

    private void RemoveButtonListener(ButtonUI button, UnityAction listener)
    {
        if (button == null || button.InteractElement == null)
        {
            return;
        }

        button.InteractElement.onClick.RemoveListener(listener);
    }

    private void SetButtonText(ButtonUI button, string text)
    {
        if (button == null)
            return;

        if (button.Text == null || text == null)
        {
            button.gameObject.SetActive(false);
        }
        else
        {
            button.gameObject.SetActive(true);
        }

        button.Text.text = text;
    }

    public async UniTask<bool> Show(string message, string confirmButtonText, string cancelButtonText)
    {
        var utcs = new UniTaskCompletionSource<bool>();

        if (_isBusy)
        {
            await UniTask.WaitUntil(() => !_isBusy);
        }

        ModalResult = null;

        Message.text = message;

        SetButtonText(ConfirmButton, confirmButtonText);
        SetButtonText(CancelButton, cancelButtonText);

        if (cancelButtonText == null || String.IsNullOrEmpty(cancelButtonText))
        {
            ConfirmButton.transform.localPosition = new Vector3(0, -76, 0);
        }
        else
        {
            ConfirmButton.transform.localPosition = new Vector3(-150, -76, 0);
            CancelButton.transform.localPosition = new Vector3(150, -76, 0);
        }

        SetLookRotation();
        ModalPanel.SetActive(true);

        _isBusy = true;

        await UniTask.WaitUntil(() => ModalResult != null);

        _isBusy = false;

        ModalPanel.SetActive(false);

        utcs.TrySetResult(ModalResult.Value);
        return await utcs.Task;
    }

    public async UniTask<bool> Show(string message, string confirmButtonText)
    {
        ConfirmButton.transform.localPosition = new Vector3(0, 76, 0);
        return await Show(message, confirmButtonText, null);
    }

    public async UniTask<bool> Show(string message)
    {
        return await Show(message, null, null);
    }

    private void OnConfirmButtonClick()
    {
        ModalResult = true;
    }

    private void OnCancelButtonClick()
    {
        ModalResult = false;
    }

    private void SetLookRotation()
    {
        if (_parentRotator != null)
        {
            _parentRotator.SetFirstPosition();
            _parentRotator.SetPlayerFollow(true);
        }
    }
}