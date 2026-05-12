using Assets.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : ViewPanel
{
    public static NotificationPanel instance;
    #region Inspector
    public TMPro.TMP_Text notificationText;
    public float timer = 1.5f;

    [SerializeField]
    private Button button;
    [SerializeField]
    private TMPro.TMP_Text buttonText;

    [SerializeField] 
    private LocalizationVariant defaultLoadingSceneMessageLocalizationVariant;
    #endregion

    public event Action ButtonClicked;

    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    public void SetInfo(string info) 
    {
        notificationText.text = info;
    }

    public void SetDefaultInfo()
    {
        notificationText.text = defaultLoadingSceneMessageLocalizationVariant.Localize().Replace("{0}","");
    }

    public override void ShowPanel()
    {
        HidePanel();
        StartCoroutine(HidePanelAfterTimer());
        base.ShowPanel();
    }

    public override void HidePanel()
    {
        button.gameObject.SetActive(false);
        base.HidePanel();
    }

    public void ShowButton(string label)
    {
        buttonText.text = label;
        button.gameObject.SetActive(true);
    }

    private IEnumerator HidePanelAfterTimer() 
    {
        yield return new WaitForSeconds(timer);

        SetDefaultInfo();
        HidePanel();
    }

    private void OnButtonClick()
    {
        ButtonClicked?.Invoke();
    }
}
