using UnityEngine;
using Assets.Scripts.Shared;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System;
using Assets.Scripts.Environmental;
using Assets.Mechanics.TextToSpeech;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Data;

namespace Assets.Scripts.Gallery.UI
{
    public class GalleryPicturePanel : MonoBehaviour
    {
        [Space] [Header("Picture stand")]
        [SerializeField] private Transform _pictureWithBorder;
        [SerializeField] private Transform _border;
        [SerializeField] private RectTransform _pasparty;
        [SerializeField] private RectTransform _image;
        [SerializeField] private NetworkTriggerObserver _playerDetector;

        [Space] [Header("Picture description")] 
        [SerializeField] private RectTransform _descriptionPanel;
        [SerializeField] private TMP_Text _author;
        [SerializeField] private GameObject _separator1;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private GameObject _separator2;
        [SerializeField] private TMP_Text _description;

        [Space] [Header("Error")]
        [SerializeField] private RectTransform _errorPanel;
        [SerializeField] private TMP_Text _errorMessage;

        [Space] [Header("Text to speech player")]
        [SerializeField] private SimpleTextToSpeechPlayer _textToSpeechPlayer;

        private float _width = 1;
        private float _height = 1;
        private float _passepartoutWidth = 0.1f;
        private float _passepartoutHeight = 0.1f;

        private Vector2 _startBorderScale;
        private Vector2 _startPaspartyScale;
        private Vector2 _startImageScale;
        private float _startDescriptionShiftX;

        private LookAtPlayer _lookAtPlayer;

        private RawImage _rawImage;
        private WebClient _web = new();
        private TowerPicture _towerPicture;
        private float _aspectRatio = 1;

        private Action ImageDownloadedCallback;

        private Canvas _pictureDescriptionCanvas;

        private void Awake()
        {
            SetStartScales();

            _lookAtPlayer = _descriptionPanel.GetComponent<LookAtPlayer>();

            _rawImage = _image.GetComponent<RawImage>();
            _web.DownloadDataCompleted += DownloadImageComplete;

            _playerDetector.LocalClientEnterTrigger += PlayerEnterNearPicture;
            _playerDetector.LocalClientExitTrigger += PlayerExitNearPicture;

            _pictureDescriptionCanvas = _descriptionPanel.GetComponent<Canvas>();
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
            ImageDownloadedCallback?.Invoke();

            if (e.Error == null)
            {
                byte[] raw = e.Result;
                DrawImage(raw);
            }
        }

        private void DrawImage(byte[] raw)
        {
            Texture2D texture2D = new Texture2D(1, 1);

            _rawImage.color = Color.white;

            if (texture2D.LoadImage(raw))
            {
                _rawImage.texture = texture2D;
                _aspectRatio = ((float)texture2D.width) / ((float)texture2D.height);

                SetDimentions();
            }
        }

        private void SetDimentions()
        {
            _width = (float)_towerPicture.PictureWidthInMeters;
            if (_width == 0)
            {
                _errorPanel.gameObject.SetActive(true);
                _errorMessage.text = "Null picture width. Check image data";
            }

            _passepartoutWidth = (float)_towerPicture.PassepartoutWidthInMeters;
            _height = _width / _aspectRatio;
            _passepartoutHeight = _passepartoutWidth / _aspectRatio;

            UpdateDimentions();
        }

        private void PlayerEnterNearPicture(GameObject player)
        {
            _descriptionPanel.gameObject.SetActive(true);
            _lookAtPlayer.SetPlayer(player.transform);
        }

        private void PlayerExitNearPicture()
        {
            _descriptionPanel.gameObject.SetActive(false);
        }


        private void SetStartScales()
        {
          _startBorderScale = new Vector2(_border.localScale.x, _border.localScale.z);
          _startPaspartyScale = new Vector2(_pasparty.localScale.x, _pasparty.localScale.y);
          _startImageScale = new Vector2(_image.localScale.x, _image.localScale.y);
          _startDescriptionShiftX = _descriptionPanel.localPosition.x - transform.localPosition.x;
        }


        private void FixedUpdate()
        {
            //SetDimentions();
            //UpdateDimentions();
        }

        private void UpdateDimentions()
        {
            _border.transform.localScale = new Vector3(
                _startBorderScale.x * (_width + _passepartoutWidth * 2),
                _border.transform.localScale.y,
                _startBorderScale.y * (_height + _passepartoutHeight * 2));

            _pasparty.transform.localScale = new Vector3(
                _startPaspartyScale.x * (_width + _passepartoutWidth * 2),
                _startPaspartyScale.y * (_height + _passepartoutHeight * 2),
                _pasparty.transform.localScale.z);

            _image.transform.localScale = new Vector3(
                _startImageScale.x * _width,
                _startImageScale.y * _height,
                _image.transform.localScale.z);

            _descriptionPanel.localPosition = new Vector3(
                (transform.localPosition.x + _startDescriptionShiftX - 0.5f + _width/2 + _passepartoutWidth),
                _descriptionPanel.localPosition.y,
                _descriptionPanel.localPosition.z);

            _pictureWithBorder.transform.localPosition = new Vector3(_pictureWithBorder.transform.localPosition.x, (_height/2 - 0.5f + _passepartoutHeight), _pictureWithBorder.transform.localPosition.z);
        }

        public void SetPicture(TowerPicture picture, Action callback)
        {
            ImageDownloadedCallback = callback;
            
            _towerPicture = picture;
            DownloadImage(picture.Image);

            SetDescription();
        }
        
        private void SetDescription()
        {
            _author.text = _towerPicture.Author.StringConvertionToGalleryDescription();
            _name.text = _towerPicture.Name.StringConvertionToGalleryDescription();
            _description.text = _towerPicture.Description.StringConvertionToGalleryDescription();

            if (!String.IsNullOrEmpty(_towerPicture.Author) 
                || !String.IsNullOrEmpty(_towerPicture.Name)
                || !String.IsNullOrEmpty(_towerPicture.Description))
            {
                _pictureDescriptionCanvas.enabled = true;
            }            
            else
            {
                _pictureDescriptionCanvas.enabled = false;
            }            

            if (!String.IsNullOrEmpty(_towerPicture.Author)) 
            {
                _author.gameObject.SetActive(true);

                if (!String.IsNullOrEmpty(_towerPicture.Name))
                {
                    _name.gameObject.SetActive(true);
                    _separator1.SetActive(true);

                    if (!String.IsNullOrEmpty(_towerPicture.Description)) // 1 - 1 - 1 // 1 - 1 - 0
                    {
                        _description.gameObject.SetActive(true);
                        _separator2.SetActive(true);
                    }
                }
                else if (!String.IsNullOrEmpty(_towerPicture.Description)) //1 - 0 - 1 //1 - 0 - 0
                {
                    _description.gameObject.SetActive(true);
                    _separator2.SetActive(true);
                }
            }
            else if (!String.IsNullOrEmpty(_towerPicture.Name)) 
            {
                _name.gameObject.SetActive(true);

                if (!String.IsNullOrEmpty(_towerPicture.Description)) //0 - 1 - 1 //0 - 1 - 0
                {
                    _description.gameObject.SetActive(true);
                    _separator2.SetActive(true);
                }
            }
            else if (!String.IsNullOrEmpty(_towerPicture.Description)) //0 - 0 - 1 
            {
                _description.gameObject.SetActive(true);
            }
            else //0 - 0 - 0
            {
                 //_descriptionPanel.gameObject.SetActive(false);
            }

            List<string> textBlocks = new();
            if (!string.IsNullOrEmpty(_towerPicture.Author))
            {
                textBlocks.Add(_towerPicture.Author);
            }
            if (!string.IsNullOrEmpty(_towerPicture.Name))
            {
                textBlocks.Add(_towerPicture.Name);
            }
            if (!string.IsNullOrEmpty(_towerPicture.Description))
            {
                textBlocks.Add(_towerPicture.Description);
            }

            _textToSpeechPlayer.Init(textBlocks);
        }
    }
}
