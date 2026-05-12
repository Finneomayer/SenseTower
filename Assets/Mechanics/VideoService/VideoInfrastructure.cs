using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.Mechanics.TextToSpeech;
using Assets.Scripts.Data;
using Assets.Scripts.Environmental;
using Assets.Scripts.Utils;
using Mechanics.VideoService.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Mechanics.VideoService
{
    public class VideoInfrastructure : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private RawImage _thubinImage;
        [SerializeField] private NetworkTriggerObserver _networkTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _button;
        [SerializeField] private Sprite _startVideoImage;
        [SerializeField] private Sprite _playVideoImage;
        [SerializeField] private GameObject _videoView;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private HandButton _handButton;
        [SerializeField] private Button _uiButton;
        [SerializeField] private TMP_Text _buttonDescription;
        [SerializeField] private Image _videoButtonImage;
        [SerializeField] private Sprite _playImage;
        [SerializeField] private Sprite _pauseImage;
        [SerializeField] private GameObject _bottomPoint;
        [SerializeField] private GameObject _leftPoint;

        [Space] [Header("Picture description")] [SerializeField]
        private RectTransform _descriptionPanel;

        [SerializeField] private TMP_Text _name;
        [SerializeField] private GameObject _separator2;
        [SerializeField] private TMP_Text _description;

        [Space] [Header("Text to speech player")] [SerializeField]
        private SimpleTextToSpeechPlayer _textToSpeechPlayer;

        #endregion

        public GameObject VideoView => _videoView;
        private VideoSpaceObject _videoSpaceObject;

        private bool _isPlaying = false;
        private double _stopTime = 0;
        private Coroutine _pauseCoroutine;
        private Coroutine _resumeCoroutine;
        private bool _executingPause = false;

        private LookAtPlayer _lookAtPlayer;
        private Canvas _pictureDescriptionCanvas;
        private Texture2D _texture2D;
        private Sprite _thumbnail;
        private WebClient _web;
        private float _aspectRatio = 0;

        private void OnEnable()
        {
            _uiButton.onClick.AddListener(OnButtonPressHandler);
            //_handButton.OnPressPhysicsButton += OnButtonPressHandler;
            _networkTrigger.LocalClientEnterTrigger += OnLocalPlayerTriggerEnter;
            _networkTrigger.LocalClientExitTrigger += OnLocalPlayerTriggerExit;
            _videoPlayer.enabled = false;
        }

        private void OnDisable()
        {
            _uiButton.onClick.RemoveListener(OnButtonPressHandler);
            //_handButton.OnPressPhysicsButton -= OnButtonPressHandler;
            _networkTrigger.LocalClientEnterTrigger -= OnLocalPlayerTriggerEnter;
            _networkTrigger.LocalClientExitTrigger -= OnLocalPlayerTriggerExit;
        }

        private void Awake()
        {
            _web = new();
            _lookAtPlayer = _descriptionPanel.GetComponent<LookAtPlayer>();
            _pictureDescriptionCanvas = _descriptionPanel.GetComponent<Canvas>();

            _web.DownloadDataCompleted += DownloadImageComplete;
        }

        private void OnDestroy()
        {
            if (_texture2D != null)
            {
                Destroy(_texture2D);
            }

            if (_thumbnail != null)
            {
                Destroy(_thumbnail);
            }
        }

        public void Init(VideoSpaceObject videoSpaceObject)
        {
            _isPlaying = false;
            _videoSpaceObject = videoSpaceObject;
            name = _videoSpaceObject.TowerObject.VideoRecord.Name;
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = _videoSpaceObject.TowerObject.VideoRecord.VideoUrl;
            _videoPlayer.Prepare();
            _videoButtonImage.sprite = _playImage;
            //_buttonDescription.text = "Воспроизвести";
            Debug.Log(_videoSpaceObject.TowerObject.VideoRecord.ThumbnailUrl);
            if (Uri.TryCreate(_videoSpaceObject.TowerObject.VideoRecord.ThumbnailUrl, UriKind.Absolute, out Uri path))
            {
                _web.DownloadDataAsync(path);
            }
            else
            {
                _spriteRenderer.sprite = _startVideoImage;
                SetPosition();
            }

            if (videoSpaceObject.TowerObject.PlayerSettings != null &&
                !videoSpaceObject.TowerObject.PlayerSettings.EnableTextToSpeech)
            {
                _textToSpeechPlayer.gameObject.SetActive(false);
            }

            SetDescription();
        }

        public void Hide()
        {
            SmoothlyPauseOverTime(_videoPlayer, 0.5f);
            _videoView.gameObject.SetActive(false);
            //_handButton.gameObject.SetActive(false);
            _mainCanvas.gameObject.SetActive(false);
            _button.SetActive(false);
        }

        private void OnButtonPressHandler()
        {
            _mainCanvas.gameObject.SetActive(false);
            if (!_isPlaying)
            {
                _spriteRenderer.sprite = _playVideoImage;
                _videoPlayer.enabled = true;
                SmoothlyResumeOverTime(_videoPlayer, 0.5f);
            }
            else
            {
                SmoothlyPauseOverTime(_videoPlayer, 0.5f);
            }

            _isPlaying = !_isPlaying;

            //_buttonDescription.text = _isPlaying ? "Остановить" : "Воспроизвести";
            _videoButtonImage.sprite = _isPlaying ? _pauseImage : _playImage;
        }

        private void OnLocalPlayerTriggerEnter(GameObject player)
        {
            _descriptionPanel.gameObject.SetActive(true);
            _lookAtPlayer.SetPlayer(player.transform);
        }

        private void OnLocalPlayerTriggerExit()
        {
            _descriptionPanel.gameObject.SetActive(false);
        }

        private void SetDescription()
        {
            _name.text = _videoSpaceObject.TowerObject.VideoRecord.Name.StringConvertionToGalleryDescription();
            _description.text = _videoSpaceObject.TowerObject.Description
                .StringConvertionToGalleryDescription();

            if (!String.IsNullOrEmpty(_videoSpaceObject.TowerObject.VideoRecord.Name)
                || !String.IsNullOrEmpty(_videoSpaceObject.TowerObject.Description))
            {
                _pictureDescriptionCanvas.enabled = true;
            }
            else
            {
                _pictureDescriptionCanvas.enabled = false;
            }

            if (!String.IsNullOrEmpty(_videoSpaceObject.TowerObject.VideoRecord.Name))
            {
                _name.gameObject.SetActive(true);

                if (!String.IsNullOrEmpty(_videoSpaceObject.TowerObject
                        .Description)) // 1 - 1 - 1 // 1 - 1 - 0
                {
                    _description.gameObject.SetActive(true);
                    _separator2.SetActive(true);
                }
            }
            else if (!String.IsNullOrEmpty(_videoSpaceObject.TowerObject.Description))
            {
                _name.gameObject.SetActive(true);
            }

            List<string> textBlocks = new();

            if (!string.IsNullOrEmpty(_videoSpaceObject.TowerObject.VideoRecord.Name))
            {
                textBlocks.Add(_videoSpaceObject.TowerObject.VideoRecord.Name);
            }

            if (!string.IsNullOrEmpty(_videoSpaceObject.TowerObject.Description))
            {
                textBlocks.Add(_videoSpaceObject.TowerObject.Description);
            }

            _textToSpeechPlayer.Init(textBlocks);
        }

        private void SetPosition()
        {
            Vector3 position =
                DataExtensions.CalculateMiddlePosition(DataExtensions.SpaceObjectToStaticObject(_videoSpaceObject));
            Quaternion rot = DataExtensions.CalculateRotation(VideoView.transform,
                DataExtensions.SpaceObjectToStaticObject(_videoSpaceObject));
            Vector3 scale = DataExtensions.CalculateScale(DataExtensions.SpaceObjectToStaticObject(_videoSpaceObject));
            VideoView.transform.position = position;
            VideoView.transform.rotation = rot;
            VideoView.transform.localScale = scale;

            _button.transform.position = new Vector3(VideoView.transform.position.x,
                _bottomPoint.transform.position.y - 0.2f, VideoView.transform.position.z);
            _button.transform.localRotation = VideoView.transform.localRotation;

            _networkTrigger.gameObject.transform.position = _button.transform.position;
            _networkTrigger.transform.localRotation = VideoView.transform.localRotation;
            _descriptionPanel.transform.rotation = VideoView.transform.rotation;
            //_descriptionPanel.transform.position = CalculateRightmostPositionVer2(_spriteRenderer.gameObject);
            //PlaceObjectToRight(VideoView.gameObject,_descriptionPanel.gameObject,_spriteRenderer ,Vector3.zero);
            PlaceObjectToRight(_descriptionPanel.gameObject);
        }

        public Vector3 offset;

        private void PlaceObjectToRight(GameObject target)
        {
            Vector3 scaledPosition = VideoView.transform.localScale / 2;
            Vector3 resultPositionVector =
                VideoView.transform.position +
                (-VideoView.transform.forward * (scaledPosition.z + offset.z))
                + (-VideoView.transform.right * offset.x);
            target.transform.position = resultPositionVector;
            
        }

        private void DownloadImageComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] raw = e.Result;
                DrawImage(raw);
            }

            SetPosition();
        }

        private void DrawImage(byte[] raw)
        {
            _spriteRenderer.color = Color.white;
            _texture2D = new(1, 1);
            if (_texture2D.LoadImage(raw))
            {
                //_texture2D = Helpers.ResizeTexture(_texture2D, new Vector2(256, 256));
                //_thumbnail = Sprite.Create(_texture2D);
                //_aspectRatio = _thumbnail.bounds.size.x / _thumbnail.bounds.size.y;
                _thubinImage.texture = _texture2D;
                //_spriteRenderer.sprite = _thumbnail;
            }
        }

        private void SmoothlyPauseOverTime(VideoPlayer targetVp, float duration)
        {
            //Stop old coroutines before starting a new one
            if (_pauseCoroutine != null)
                StopCoroutine(_pauseCoroutine);

            if (_resumeCoroutine != null)
                StopCoroutine(_resumeCoroutine);


            _executingPause = true;
            _pauseCoroutine = StartCoroutine(SmoothlyPauseOverTimeCOR(targetVp, duration));
        }

        private void SmoothlyResumeOverTime(VideoPlayer targetVp, float duration)
        {
            if (_pauseCoroutine != null)
                StopCoroutine(_pauseCoroutine);

            //Stop old coroutines before starting a new one
            if (_resumeCoroutine != null)
                StopCoroutine(_resumeCoroutine);

            _resumeCoroutine = StartCoroutine(SmoothlyResumeOverTimeCOR(targetVp, duration));
        }

        private IEnumerator SmoothlyPauseOverTimeCOR(VideoPlayer targetVp, float duration)
        {
            float counter = 0;
            //Get the current playbackSpeed of the VideoPlayer 
            float startSpeed = targetVp.playbackSpeed;

            //We want to go to 0 but within duration
            float endSpeed = 0;

            //Normal speed to slow speed
            while (counter < duration)
            {
                counter += Time.deltaTime;
                targetVp.playbackSpeed = Mathf.Lerp(startSpeed, endSpeed, counter / duration);
                yield return null;
            }

            //Now, do the actual pause
            targetVp.Pause();
            _executingPause = false;
        }

        private IEnumerator SmoothlyResumeOverTimeCOR(VideoPlayer targetVp, float duration)
        {
            float counter = 0;
            //Get the current playbackSpeed of the VideoPlayer 
            //float startSpeed = targetVp.playbackSpeed;
            float startSpeed = 0f;

            //We want to go to 1 but within duration
            float endSpeed = 1;


            //Do the actual resume
            targetVp.Play();

            //Slow speed to normal Speed
            while (counter < duration)
            {
                counter += Time.deltaTime;
                targetVp.playbackSpeed = Mathf.Lerp(startSpeed, endSpeed, counter / duration);
                yield return null;
            }
        }
    }
}