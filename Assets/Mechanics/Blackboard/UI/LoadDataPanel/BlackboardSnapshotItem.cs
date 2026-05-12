using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

namespace Assets.Blackboard
{
    public sealed class BlackboardSnapshotItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text SnapshotNameText;
        [SerializeField]
        private Image ThumbnailImage;
        [SerializeField]
        private ButtonUI LoadButton;
        [SerializeField]
        private ButtonUI DeleteButton;

        private BlackboardEventMediator _blackboardEventMediator;
        private string _filename;
        private Texture2D _currentThumbnailTexture;

        public string Filename => _filename;

        private void Awake()
        {
            ThumbnailImage.enabled = false;
        }

        private void OnEnable()
        {
            LoadButton.InteractElement.onClick.AddListener(OnLoadButtonClick);
            DeleteButton.InteractElement.onClick.AddListener(OnDeleteButtonClick);
        }

        private void OnDisable()
        {
            LoadButton.InteractElement.onClick.RemoveListener(OnLoadButtonClick);
            DeleteButton.InteractElement.onClick.RemoveListener(OnDeleteButtonClick);
        }

        private void OnDestroy()
        {
            if (_currentThumbnailTexture != null)
            {
                DestroyImmediate(_currentThumbnailTexture);
            }
        }

        public void Init(BlackboardEventMediator blackboardEventMediator,
            string filename, byte[] thumbnailData)
        {
            _filename = filename;
            _blackboardEventMediator = blackboardEventMediator;

            string blackboardName = filename;
            int fileExtFirstCharIndex = blackboardName.LastIndexOf('.');
            if (fileExtFirstCharIndex > 0)
            {
                blackboardName = blackboardName.Remove(fileExtFirstCharIndex);
            }

            int shortFileNameFirstCharIndex = blackboardName.LastIndexOf(Path.DirectorySeparatorChar);
            if (shortFileNameFirstCharIndex == -1)
            {
                shortFileNameFirstCharIndex = blackboardName.LastIndexOf(Path.AltDirectorySeparatorChar);
            }
            if (shortFileNameFirstCharIndex > 0)
            {
                blackboardName = blackboardName.Remove(0, shortFileNameFirstCharIndex + 1);
            }

            SnapshotNameText.text = blackboardName;

            SetImage(thumbnailData);

        }

        private void SetImage(byte[] rawData)
        {
            if (_currentThumbnailTexture != null)
            {
                _currentThumbnailTexture.Reinitialize(1, 1);
            }
            else
            {
                _currentThumbnailTexture  = new Texture2D(1, 1);
            }

            if (_currentThumbnailTexture.LoadImage(rawData))
            {
                ThumbnailImage.sprite = Sprite.Create(_currentThumbnailTexture,
                    new Rect(0, 0, _currentThumbnailTexture.width, _currentThumbnailTexture.height), new Vector2(0, 1));
                ThumbnailImage.enabled = true;
            }
        }

        private void OnLoadButtonClick()
        {
            _blackboardEventMediator.RequestLoadBlackboardFile(_filename);
        }

        private void OnDeleteButtonClick()
        {
            _blackboardEventMediator.RequestDeleteBlackboardFile(_filename);
        }
    }
}


