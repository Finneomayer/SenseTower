using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.Localization;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PictureItemUI : MonoBehaviour
{
    [SerializeField] private Image _selected;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Button _button;
    [SerializeField] private LocalizationVariant _imageDoorLabelLocalizationVariant;
    [SerializeField] private LocalizationVariant _imageWallLabelLocalizationVariant;

    public Action OnClick;

    public void SetImage(MyImage image, bool isDoorImage = false)
    {
        MyImageDownloader myImageDownloader = new();
        myImageDownloader.Init();
        myImageDownloader.SetPreviewTexture(image, _image);

        _name.text = isDoorImage == true ? GetFormattedLabel(_imageDoorLabelLocalizationVariant.Localize()) 
            : image.Name;
        _button.onClick.AddListener(() => OnClick?.Invoke());
    }

    public void SetImage(bool isDoorImage = false)
    {
        string labelText = isDoorImage == true ? _imageDoorLabelLocalizationVariant.Localize()
            : _imageWallLabelLocalizationVariant.Localize();
        _name.text = GetFormattedLabel(labelText);
        _button.onClick.AddListener(() => OnClick?.Invoke());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Select(bool flag)
    {
        _selected.enabled = flag;
    }

    private string GetFormattedLabel(string labelText)
    {
        return $"[{labelText}]";
    }
}
