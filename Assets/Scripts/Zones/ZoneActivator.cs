using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UI;
using UnityEngine.EventSystems;

public class ZoneActivator : MonoBehaviour, IPointerClickHandler
{
    #region Inspector
    [SerializeField] private ViewPanel _zonePanel;
    [SerializeField] private XRSimpleInteractable _xrInteractable;
    [SerializeField] private bool _setUIObjectActiveFalseWhenHidden = false;
    public bool CanInteract = false;
    #endregion

    #region Private Variables
    private LookAtPlayer lookAt;
    private bool _isVisible = false;
    #endregion

    private void Awake()
    {
        lookAt = _zonePanel.GetComponent<LookAtPlayer>();
    }

    private void OnEnable()
    {
        _xrInteractable.activated.AddListener(delegate { ToggleUI(); });
        _zonePanel.PanelBlockedStatusChanged += _zonePanel_PanelBlockedStatusChanged;
    }

    private void _zonePanel_PanelBlockedStatusChanged(bool flag)
    {
        _zonePanel.gameObject.SetActive(!flag);
        _isVisible = !flag;
        _xrInteractable.gameObject.SetActive(!flag);
    }

    private void OnDisable()
    {
        _xrInteractable.activated.RemoveAllListeners();
        _zonePanel.PanelBlockedStatusChanged -= _zonePanel_PanelBlockedStatusChanged;
    }

    public void SetLookTransform(Transform lookTransform)
    {
        lookAt.SetPlayer(lookTransform);
    }

    public void HidePanel() 
    {
        lookAt.DeletePlayer();
        _zonePanel.HidePanel();
        _isVisible = false;
    }

    private void ToggleUI()
    {
        if (!CanInteract) return;
        if (_zonePanel.PanelBlocked) return;

        _isVisible = !_isVisible;

        if (_isVisible)
        {
            if (_setUIObjectActiveFalseWhenHidden) _zonePanel.gameObject.SetActive(true);
            else _zonePanel.ShowPanel();
        }
        else
        {
            if (_setUIObjectActiveFalseWhenHidden) _zonePanel.gameObject.SetActive(false);
            else HidePanel();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleUI();
    }
}
