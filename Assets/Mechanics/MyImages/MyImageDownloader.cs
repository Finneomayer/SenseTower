using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyImageDownloader
{
    private Image _sprite;
    private string _debugNumber;
    Texture2D texture2D;
    private string downloadUrl = string.Empty;
    private bool _isDownloadComplete = false;
    public void Init() 
    {
        texture2D = new Texture2D(1, 1);
    }
    
    public void SetPreviewTexture(MyImage myImage, Image sprite)
    {
        _sprite = sprite;
        SetTexture(myImage.PreviewUrl);
    }

    public void SetPreviewTexture(MyImage myImage, Image sprite, string debugNumber)
    {
        _sprite = sprite;
        _debugNumber = debugNumber;
        SetTexture(myImage.PreviewUrl);
    }

    public  void SetFullTexture(MyImage myImage, Image sprite, string debugNumber)
    {
        _sprite = sprite;
        _debugNumber = debugNumber;

        SetTexture(myImage.FileUrl);
    }

    private  void SetTexture(string url)
    {
        if (texture2D == null) return;
        if (string.IsNullOrEmpty(url)) return;
        if (url == downloadUrl && _isDownloadComplete) return;

        _isDownloadComplete = false;

        WebClient web = new WebClient();
        web.DownloadDataCompleted += DownloadImageComplete;

        if (Uri.TryCreate(url, UriKind.Absolute, out Uri path))
        {
            web.DownloadDataAsync(path);
            downloadUrl = url;
        }
    }

    private  void DownloadImageComplete(object sender, DownloadDataCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            byte[] raw = e.Result;
            DrawImage(raw);
        }
        else
        {
            //Debug.LogWarning(e.Error.Message + "Door number" + _debugNumber);
        }
    }

    private  void DrawImage(byte[] raw)
    {
        if (texture2D.LoadImage(raw))
        {
            if (_sprite == null)
                return;
            
            _sprite.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one / 2);
            if (texture2D.width > texture2D.height)
            {
                var width = _sprite.rectTransform.rect.width;
                var height = width * texture2D.height / texture2D.width;
                _sprite.rectTransform.sizeDelta = new Vector2(width, height);
            }
            else
            {
                var height = _sprite.rectTransform.rect.height;
                var width = height / texture2D.height * texture2D.width;
                _sprite.rectTransform.sizeDelta = new Vector2(width, height);
            }
            _isDownloadComplete = true;
        }
    }
}
