using Assets.Mechanics.Browser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using Vuplex.WebView;

[Serializable]
public class MainPresentationForPad
{
    public int BrowserNum;
    public CanvasWebViewPrefab MainBrowserWebView;
    public Presentation MainPresentation;
}

[RequireComponent(typeof(PadButtons))]
[RequireComponent(typeof(PadKeyboard))]
[RequireComponent(typeof(PadSearch))]
public class PadSwitcher : MonoBehaviour
{
    [SerializeField] private bool _canConnectPad;
    [Header("Input")] 
    public List<MainPresentationForPad> Browsers;

    [SerializeField] private Transform _parentForCanvasWebView;
    [SerializeField] private GameObject _canvasWebViewPrefab;
    private Queue<GameObject> _canvasWebViews;
    [Header("Output")]
    [SerializeField] private GameObject _pad;    
    //private CanvasWebViewPrefab _padBrowserWebView;
    [SerializeField] private GameObject _padBrowser;
    [SerializeField] private ViewPanel _searchPanel;
    [SerializeField] private List<WebPageActionsSlot> _webPageActionSlots = new();
    [SerializeField] private CanvasPointerInputDetector _padInputDetector;

    //[HideInInspector]
    public bool SearchFieldIsVisible;

    //for network pad to close from big browser close button
    public event Action CloseBrowser;

    private Browser _currentBrowserForPad;

    private PadButtons _padButtons;
    private PadKeyboard _padKeyboard;
    private PadSearch _padSearch;

    private Vector3 _padPosition;
    private Vector3 _padScale;
    private Quaternion _padRotation;

    private void Awake()
    {
        Browsers = new List<MainPresentationForPad>();
        _canvasWebViews = new Queue<GameObject>();

        //if(_presentation == null)
        //    _presentation = FindObjectOfType<Presentation>();
        //if (_presentation != null)
        //{
        //    //foreach (WebPageActionsSlot webPageActionsSlot in _webPageActionSlots)
        //    //{
        //    //    _presentation.AddNewWebPageSlot(webPageActionsSlot);
        //    //}
        //    //_presentation.BrowserShownToAdmin += Show;
        //    //_presentation.BrowserHiddenToAdmin += Hide;
        //}

        //to initialize components when pad is shown
        _padButtons = GetComponent<PadButtons>();
        _padKeyboard = GetComponent<PadKeyboard>();
        _padSearch = GetComponent<PadSearch>();
        //if (_mainBrowserWebView != null)
        //{
        //    InitPadBrowserAfterMain();
        //}
    }

    public PadButtons GetButtons()
    {
        return _padButtons;
    }

    private void OnDestroy()
    {
        foreach (var browser in Browsers)
        {
            browser.MainPresentation.BrowserShownToAdmin -= Show;
            browser.MainPresentation.BrowserHiddenToAdmin -= Hide;
        }
    }

    public void ShowWithUrl(string url)
    {
        Show();
        _currentBrowserForPad.SetUrl(url);
    }

    public void Show()
    {
        _padButtons.UnsubscribeButtons();
        _padKeyboard.UnsubscribeKeyboard();
        _padSearch.UnsubscribeSearch();
        UnregisterPadInputListerers();

        _pad.SetActive(true);
        _padButtons.InitButtons();
        _padKeyboard.InitKeyboardOfLocalPad();
        //_padSearch.InitSearch(SearchFieldIsVisible, _padBrowserWebView, _padInputDetector);
        RegisterPadInputListerers();
        //_pad.transform.position = _padPosition;
        //_pad.transform.rotation = _padRotation;
        //_pad.transform.localScale = _padScale;
    }

    public void Hide()
    {
        _padButtons.UnsubscribeButtons();
        _padKeyboard.UnsubscribeKeyboard();
        _padSearch.UnsubscribeSearch();
        _padSearch.HideSearchPanel();
        CloseBrowser?.Invoke();
        _pad.SetActive(false);

        UnregisterPadInputListerers();
    }

    public GameObject GetPad()
    {
        return _pad;
    }

    public void AddBrowser(Browser browser, Presentation presentation)
    {
        foreach (WebPageActionsSlot webPageActionsSlot in _webPageActionSlots)
        {
            presentation.AddNewWebPageSlot(webPageActionsSlot);
        }
        presentation.BrowserShownToAdmin += Show;
        presentation.BrowserHiddenToAdmin += Hide;

        MainPresentationForPad item = new MainPresentationForPad()
        {
            BrowserNum = browser.BrowserNumber.Value,
            MainBrowserWebView = browser.WebViewPrefab,
            MainPresentation = presentation
        };

        Browsers.Add(item);

        //if (Browsers.Count == 1) InitPadBrowserAfterMain(Browsers[0]);
    }

    //public void SetBrowser(Browser browser)
    //{
    //    _mainBrowserWebView = browser.WebViewPrefab;
    //    _padKeyboard.SetBrowser(browser);

    //    InitPadBrowserAfterMain();
    //}

    public async void InitPadBrowserAfterMain(MainPresentationForPad browser)
    {
        await browser.MainBrowserWebView.WaitUntilInitialized();

        _padBrowser.SetActive(true);

        //if (_padBrowserWebView != null) Destroy(_padBrowserWebView.gameObject);
        GameObject webView = Instantiate(_canvasWebViewPrefab, _parentForCanvasWebView);
        //_canvasWebViews.Enqueue(webView);
        var padBrowserWebView = webView.GetComponent<CanvasWebViewPrefab>();
        _padInputDetector = padBrowserWebView.GetComponentInChildren<CanvasPointerInputDetector>();
        Debug.LogError(_padInputDetector == null);
        //await UniTask.Delay(1000);
        //await UniTask.WaitUntil(() => browser.MainBrowserWebView.WebView != null);
        if (browser.MainBrowserWebView.WebView != null)
        {
            padBrowserWebView.SetWebViewForInitialization(browser.MainBrowserWebView.WebView);
        }

        _currentBrowserForPad = browser.MainPresentation.Browser;
        _padSearch.InitSearch(SearchFieldIsVisible, padBrowserWebView, _padInputDetector);

        //if (_canvasWebViews.Count > 2) Destroy(_canvasWebViews.Dequeue());

    }
    
    private void RegisterPadInputListerers()
    {
        if (_padInputDetector == null) return;
        _padInputDetector.BeganDrag += OnLocalInputBeganDrag;
        _padInputDetector.Dragged += OnLocalInputDragged;
        _padInputDetector.PointerDown += OnLocalInputPointerDown;
        _padInputDetector.PointerExited += OnLocalInputPointerExited;
        _padInputDetector.PointerUp += OnLocalInputPointerUp;
        _padInputDetector.Scrolled += OnLocalInputScrolled;
    }

    private void UnregisterPadInputListerers()
    {
        if (_padInputDetector == null) return;
        _padInputDetector.BeganDrag -= OnLocalInputBeganDrag;
        _padInputDetector.Dragged -= OnLocalInputDragged;
        _padInputDetector.PointerDown -= OnLocalInputPointerDown;
        _padInputDetector.PointerExited -= OnLocalInputPointerExited;
        _padInputDetector.PointerUp -= OnLocalInputPointerUp;
        _padInputDetector.Scrolled -= OnLocalInputScrolled;
    }

    private void OnLocalInputScrolled(object sender, ScrolledEventArgs e)
    {
        EnqueuePageAction(new ScrolledWebPageInputAction(e));
    }

    private void OnLocalInputPointerUp(object sender, PointerEventArgs e)
    {
        EnqueuePageAction(new PointerUpWebPageInputAction(e));
    }

    private void OnLocalInputPointerExited(object sender, EventArgs e)
    {
        EnqueuePageAction(new PointerExitedWebPageInputAction(e));
    }

    private void OnLocalInputPointerDown(object sender, PointerEventArgs e)
    {
        EnqueuePageAction(new PointerDownWebPageInputAction(e));
    }

    private void OnLocalInputDragged(object sender, EventArgs<Vector2> e)
    {
        EnqueuePageAction(new DraggedWebPageInputAction(e));
    }

    private void OnLocalInputBeganDrag(object sender, EventArgs<Vector2> e)
    {
        EnqueuePageAction(new BeganDragWebPageInputAction(e));
    }

    private void EnqueuePageAction(WebPageInputAction webPageInputAction)
    {
        foreach (var browser in Browsers)
        {
            browser.MainPresentation.Browser.NetworkWebBrowser.SendInputAction(webPageInputAction);
        }
    }
}

