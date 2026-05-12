using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerSign : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable _xrInteractable;
    [SerializeField] private LocalizationVariant _description;

    public event Action<string> OnHover;

    private void OnEnable()
    {
        _xrInteractable.hoverEntered.AddListener((args) => OnHover?.Invoke(_description.Localize()));
    }

    private void OnDestroy()
    {
        _xrInteractable.hoverEntered.RemoveAllListeners();
    }
}
