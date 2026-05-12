using System;
using System.Net;
using TMPro;
using UnityEngine;

public class PicturePanel : MonoBehaviour
{
    public int PlaceNumber { get; set; }
    
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private TMP_Text _numberView;
    private WebClient _web = new();
    private string currentFIleUrl = string.Empty;
    private Texture2D texture2D = null;

    private void Start()
    {
        texture2D = new Texture2D(1, 1);

        _web.DownloadDataCompleted += DownloadImageComplete;
    }

    public void ToogleVisibility(bool isVisible)
    {
        //_renderer.enabled = isVisible;
    }

    public void SetAndShowNumber(int num)
    {
        _numberView.text = num.ToString();
        _numberView.gameObject.SetActive(true);
    }

    public void SetTexture(MyImage picture)
    {
        if (currentFIleUrl.Contains(picture.FileUrl))
            return;

        currentFIleUrl = picture.FileUrl;
        DownloadImage(picture);
    }

    private void DownloadImage(MyImage image)
    {
        if (Uri.TryCreate(image.FileUrl, UriKind.Absolute, out Uri path))
        {
            _web.DownloadDataAsync(path);
        }
    }

    private void DownloadImageComplete(object sender, DownloadDataCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            byte[] raw = e.Result;
            DrawImage(raw);
        }
    }

    private void DrawImage(byte[] raw)
    {
        if (texture2D.LoadImage(raw))
        {
            foreach (var material in _renderer.materials)
            {
                if (material.shader.name == "Shader Graphs/Lightmapped") material.mainTexture = texture2D;
            }
        }
    }
}
