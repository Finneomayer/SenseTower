using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneExpanderView : MonoBehaviour
{
    public event Action<int> ChangeSize;

    [SerializeField] private Button _participantsBtn;
    [SerializeField] private Button _sizeBtn;
    [SerializeField] private GameObject _participantsTab;
    [SerializeField] private GameObject _sizeTab;
    [SerializeField] private Button _standartVariantBtn;
    [SerializeField] private Button _bigVariantBtn;
    [SerializeField] private Image _standartVariantSelected;
    [SerializeField] private Image _bigVariantSelected;

    public void SelectVariant(int size)
    {
        if (size == 0)
        {
            _bigVariantSelected.enabled = false;
            _standartVariantSelected.enabled = true;
        }
        else if (size == 1)
        {
            _bigVariantSelected.enabled = true;
            _standartVariantSelected.enabled = false;
        }
    }

    private void OnEnable()
    {
        _participantsBtn.onClick.AddListener(Showparticipants);
        _sizeBtn.onClick.AddListener(ShowSizeVariants);
        _standartVariantBtn.onClick.AddListener(SetStandartSize);
        _bigVariantBtn.onClick.AddListener(SetBigtSize);
    }

    private void SetBigtSize()
    {
        ChangeSize?.Invoke(1);
    }

    private void SetStandartSize()
    {
        ChangeSize?.Invoke(0);
    }

    private void OnDisable()
    {
        _participantsBtn.onClick.RemoveListener(Showparticipants);
        _sizeBtn.onClick.RemoveListener(ShowSizeVariants);
        _standartVariantBtn.onClick.RemoveListener(SetStandartSize);
        _bigVariantBtn.onClick.RemoveListener(SetBigtSize);
    }

    private void ShowSizeVariants()
    {
        _participantsTab.SetActive(false);
        _sizeTab.SetActive(true);
    }

    private void Showparticipants()
    {
        _participantsTab.SetActive(true);
        _sizeTab.SetActive(false);
    }
}
