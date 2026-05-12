using Assets.Mechanics.Keyboard.Scripts;
using System;
using UnityEngine;
using Vuplex.WebView;

public class XrWebViewTest : MonoBehaviour 
{
    [SerializeField]
    private CanvasWebViewPrefab CanvasWebViewPrefab;
    [SerializeField]
    private KeyboardWebAdapter Keyboard;
    [SerializeField]
    private bool OverrideResolutionDependsOnSize = true;
    [SerializeField]
    private int PixelsPerUnit = 720;

    public bool IsAllowSendKeyToBrowser = true;

    private void Awake()
    {
#if UNITY_SERVER
        gameObject.SetActive(false);
        return;
#endif

        if (OverrideResolutionDependsOnSize)
        {
            var rectTransform = CanvasWebViewPrefab.transform as RectTransform;
            CanvasWebViewPrefab.Resolution = PixelsPerUnit / rectTransform.rect.width;

            rectTransform = Keyboard.transform as RectTransform;
            //Keyboard.Resolution = PixelsPerUnit / rectTransform.rect.width;
        }

        CanvasWebViewPrefab.KeyboardEnabled = false;
    }

    private void OnEnable() 
    {
        Keyboard.KeyPressed += OnKeyboardKeyPressed;
    }

    private void OnDisable()
    {
        Keyboard.KeyPressed -= OnKeyboardKeyPressed;
    }

    private void OnKeyboardKeyPressed(object sender, EventArgs<string> e)
    {
        if (IsAllowSendKeyToBrowser)
        {
            SendKey(e.Value);
        }
    }

    private async void SendKey(string key)
    {
        await CanvasWebViewPrefab.WaitUntilInitialized();
        CanvasWebViewPrefab.WebView.SendKey(key);
    }
}
