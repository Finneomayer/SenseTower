using Assets.Mechanics.Keyboard.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Vuplex.WebView;

[RequireComponent(typeof(PadButtons))]
public class PadSearch : MonoBehaviour
{
    [Header("Input field search")]
    [SerializeField] private ViewPanel _searchPanel;
    [SerializeField] private Transform _searchPanelOpened;
    [SerializeField] private TMP_InputField _searchInput;
    [Header("Browser search")]
    //[SerializeField] 
    private CanvasPointerInputDetector _inputDetector;
    //[SerializeField] private CanvasWebViewPrefab _canvasWebViewPrefab;

    //public event Action SearchOpened; //for keyboard
    //public event Action SearchClosed; //for keyboard
    public event Action InputFieldSetFocused;
    public event Action PadBrowserSetFocused;

    private PadButtons _padButtons;
    private Vector3 _searchPanelClosedPosition;
    private Vector3 _searchPanelOpenedPosition;
    private Coroutine _searchPanelMover;

    private void Awake()
    {
        _padButtons = GetComponent<PadButtons>();
        _searchInput.shouldHideSoftKeyboard = true;
    }

    private void Start()
    {
        _padButtons.SearchOpenRequest += OnSearchOpenRequest;

        _searchPanelClosedPosition = _searchPanel.transform.localPosition;
        _searchPanelOpenedPosition = _searchPanel.transform.localPosition;
    }    
    public void SyncrhronizeInputField(string text)
    {
        _searchInput.text = text;
    }

    public void InitSearch(bool isOpen, CanvasWebViewPrefab canvasWebViewPrefab, CanvasPointerInputDetector inputDetector)
    {
        _searchInput.onSelect.AddListener(SetFocusedInputField);
        _inputDetector = inputDetector;
        _inputDetector.PointerDown += InputDetector_PointerDown;
        canvasWebViewPrefab.SetPointerInputDetector(_inputDetector);
        OnSearchOpenRequest(isOpen);
    }    

    public void UnsubscribeSearch()
    {
        _searchInput.onSelect.RemoveAllListeners();
        if (_inputDetector != null) _inputDetector.PointerDown -= InputDetector_PointerDown;
    }

    private void InputDetector_PointerDown(object sender, PointerEventArgs e)
    {        
        PadBrowserSetFocused?.Invoke();
    }

    private void OnSearchOpenRequest(bool switchToOpen)
    {
        if (switchToOpen)
        {
            _searchPanel.ShowPanel();
            if (_searchPanelMover != null) StopCoroutine(_searchPanelMover);
            _searchPanelMover = StartCoroutine(PanelMoveCoroutine(_searchPanelOpenedPosition));

        }
        else
        {
            if (_searchPanelMover != null) StopCoroutine(_searchPanelMover);
            _searchPanelMover = StartCoroutine(PanelMoveCoroutine(_searchPanelClosedPosition));
            switchToOpen = false;
            _searchPanel.HidePanel();
            //SearchClosed?.Invoke();
        }
    }

    public void HideSearchPanel()
    {
        if (_searchPanelMover != null) StopCoroutine(_searchPanelMover);
        //_searchPanel.transform.localPosition = _searchPanelClosedPosition;
        _searchPanel.HidePanel();
    }

    private IEnumerator PanelMoveCoroutine(Vector3 targetPos, bool hide = false)
    {
        while (Vector3.Distance(_searchPanel.transform.localPosition, targetPos) > float.Epsilon)
        {
            _searchPanel.transform.localPosition = Vector3.MoveTowards(_searchPanel.transform.localPosition, targetPos, Time.deltaTime * 200);
            yield return null;
        }
        if (hide) _searchPanel.HidePanel();
    }

    private void SetFocusedInputField(string args)
    {        
        InputFieldSetFocused?.Invoke();
    }
}
