using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Mechanics.Lift;
using TMPro;
using UnityEngine;

public class LiftButtonsVisual : MonoBehaviour
{
    [SerializeField] private LiftButton _mainButton;
    [SerializeField] private LiftButton _balconyButton;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private MeshRenderer _baseMeshRenderer;
    [SerializeField] private Material _materialIdle;
    [SerializeField] private Material _materialLeftActive;
    [SerializeField] private Material _materialRightActive;
    [SerializeField] private LocalizationVariant _noAccess;
    [SerializeField] private LocalizationVariant _alreadyThere;

    private void OnEnable()
    {
        _mainButton.OnHoverEnter += MainButton_OnHoverEnter;
        _mainButton.OnHoverExit += Button_OnHoverExit;
        _balconyButton.OnHoverEnter += BalconyButton_OnHoverEnter;
        _balconyButton.OnHoverExit += Button_OnHoverExit;
    }

    private void OnDisable()
    {
        _mainButton.OnHoverEnter -= MainButton_OnHoverEnter;
        _mainButton.OnHoverExit -= Button_OnHoverExit;
        _balconyButton.OnHoverEnter -= BalconyButton_OnHoverEnter;
        _balconyButton.OnHoverExit -= Button_OnHoverExit;
    }

    private void MainButton_OnHoverEnter()
    {
        if (_mainButton.AlreadyOnThisFloor)
        {
            _infoText.color = Color.white;
            _infoText.text = _alreadyThere.Localize();
            return;
        }

        if (_mainButton.Blocked)
        {
            _infoText.color = Color.red;
            _infoText.text = _noAccess.Localize();
            return;
        }
        _baseMeshRenderer.material = _materialLeftActive;
    }

    private void BalconyButton_OnHoverEnter()
    {
        if (_balconyButton.AlreadyOnThisFloor)
        {
            _infoText.color = Color.white;
            _infoText.text = _alreadyThere.Localize();
            return;
        }

        if (_balconyButton.Blocked)
        {
            _infoText.color = Color.red;
            _infoText.text = _noAccess.Localize();
            return;
        }
        _baseMeshRenderer.material = _materialRightActive;
    }

    private void Button_OnHoverExit()
    {
        _baseMeshRenderer.material = _materialIdle;
        _infoText.text = "";
    }
}
