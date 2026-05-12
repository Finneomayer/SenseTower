using System;
using System.Net;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Assets.Mechanics.TextToSpeech;
using Assets.Scripts.Data;

namespace Assets.Scripts.Gallery.UI
{
    public class GalleryInfoTablePanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private RawImage _previewImage;
        [SerializeField] private ViewPanel _loadingPanel;
        [SerializeField] private GameObject _separator;
        [SerializeField] private GameObject _descriptionParent;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private SimpleTextToSpeechPlayer _textToSpeechPlayer;

        #endregion

        private WebClient _web = new();
        private GalleryInfoTable _galleryInfoTable;
        private Vector2 _startImageScale;

        private void Start()
        {
            _startImageScale = new Vector2(_previewImage.transform.localScale.x, _previewImage.transform.localScale.y);

            _web.DownloadDataCompleted += DownloadImageComplete;
        }

        private void OnDisable()
        {
            _web.DownloadDataCompleted -= DownloadImageComplete;
        }

        public void SetGalleryInfoTable(GalleryInfoTable galleryInfoTable)
        {
            _galleryInfoTable = galleryInfoTable;
            SetText();
            SetImage();
        }

        private void SetText()
        {
            if (_galleryInfoTable != null && !string.IsNullOrEmpty(_galleryInfoTable.Description))
            {
                _descriptionText.text = _galleryInfoTable.Description.StringConvertionToGalleryDescription();
                _textToSpeechPlayer.Init(_galleryInfoTable.Description);
            }
            else
            {
                _descriptionParent.SetActive(false);
                _separator.SetActive(false);
            }
        }

        private void SetImage()
        {
            if (!string.IsNullOrEmpty(_galleryInfoTable.Image.FileUrl))
            {
                _loadingPanel.ShowPanel();
                DownloadImage();
            }
            else
            {
                _separator.SetActive(false);
                _previewImage.gameObject.SetActive(false);
            }
        }

        private void DownloadImage()
        {
            if (Uri.TryCreate(_galleryInfoTable.Image.FileUrl, UriKind.Absolute, out Uri path))
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
            Texture2D texture2D = new Texture2D(1, 1);
            _loadingPanel.HidePanel();

            Color color = Color.white;
            color.a = 0.9f;

            _previewImage.color = color;
            if (texture2D.LoadImage(raw))
            {
                _previewImage.texture = texture2D;

                float _aspectRatio = texture2D.width / ((float)texture2D.height);
                float newScaleX = _startImageScale.x;
                float newScaleY = _startImageScale.y;
                if (texture2D.width > texture2D.height)
                {
                    newScaleY *= 1 / _aspectRatio;
                }
                else
                {
                    newScaleX *= _aspectRatio;
                }

                _previewImage.transform.localScale = new Vector3(newScaleX, newScaleY, _previewImage.transform.localScale.z);
            }

        }
    }
}