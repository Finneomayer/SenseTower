using System;
using System.Collections.Generic;
using Assets.Scripts.API;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlacePictureChooser : ViewPanel
{
    [SerializeField] private RectTransform _pictureList;
    [SerializeField] private MyPictureItemUI _myPictureItemPrefab;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _emptyImage;

    public event Action ImageChanged;

    private IMyImageService _myImageService;
    private IMyPlaceService _myPlaceService;
    private MyImage[] _myImages;
    private MyPictureItemUI _instantiatedPictureUiItem;
    private int _changingPictureNumber;
    private LocalSpace _changingMyPlace;
    private List<MyPictureItemUI> _pictureItemsList = new List<MyPictureItemUI>();
    private bool _isDoorImage;

    [Inject]
    public async void Init(IMyImageService myImageService, IMyPlaceService myPlaceService)
    {
        _myPlaceService = myPlaceService;

        _myImageService = myImageService;
        _myImages = await _myImageService.GetAllImages();

        if (_myImages != null)
        {
            foreach (var image in _myImages)
            {
                CreateImageItem(image);
            }
        }

        _returnButton.onClick.AddListener(HidePanel);

        _emptyImage.onClick.AddListener(EmptyImageOnClick);
    }

    public void SetChangingImage(LocalSpace myPlace, int key, bool isDoorImage = false)
    {
        _changingPictureNumber = key;
        _changingMyPlace = myPlace;
        _isDoorImage = isDoorImage;
    }

    public void Refresh()
    {
        foreach (var picture in _pictureItemsList)
        {
            picture.Select(false);
        }
    }

    private void CreateImageItem(MyImage image) //new picture
    {
        _instantiatedPictureUiItem = Instantiate(_myPictureItemPrefab, _pictureList);
        _instantiatedPictureUiItem.SetImage(image);
        _instantiatedPictureUiItem.OnClick +=() => ImageOnClick(image);
        _pictureItemsList.Add(_instantiatedPictureUiItem);
    }

    private async void ImageOnClick(MyImage image)
    {
        if (!_isDoorImage)
        {
            _changingMyPlace.Images[_changingPictureNumber] = image;

            bool hasChanged = await _myPlaceService.ReplaceAllMyPlacePictures(_changingMyPlace.Id, _changingMyPlace.Images);
            if (hasChanged)
            {
                FindObjectOfType<MyPlaceView>().RefreshMySpaceItems();
                _myPlaceService.UpdateMyPlaceData();
                Refresh();
                _instantiatedPictureUiItem.Select(true);
                HidePanel();

                ImageChanged?.Invoke(); //now uses to update Office images at runtime
            }
            else Debug.LogError("Image doesn't change");
        }
        else
        {
            _changingMyPlace.DoorImage = image;

            bool hasChanged = await _myPlaceService.UpdateDoorImage(_changingMyPlace.Id, _changingMyPlace.DoorImage.Id);
            if (hasChanged)
            {
                FindObjectOfType<MyPlaceView>().RefreshMySpaceItems();
                _myPlaceService.UpdateMyPlaceData();
                Refresh();
                _instantiatedPictureUiItem.Select(true);
                HidePanel();
            }
            else Debug.LogError("Door image doesn't change");
        }
    }

    private async void EmptyImageOnClick()
    {
        if (!_isDoorImage)
        {
            _changingMyPlace.Images.Remove(_changingPictureNumber);

            bool hasChanged = await _myPlaceService.ReplaceAllMyPlacePictures(_changingMyPlace.Id, _changingMyPlace.Images);
            if (hasChanged)
            {
                FindObjectOfType<MyPlaceView>().RefreshMySpaceItems();

                Refresh();
                _instantiatedPictureUiItem.Select(true);
                HidePanel();
            }
            else Debug.LogError("Image doesn't change to empty");
        }
        else
        {
            _changingMyPlace.DoorImage = new MyImage() { Id = Guid.Empty };

            bool hasChanged = await _myPlaceService.ResetDoorImage(_changingMyPlace.Id);
            if (hasChanged)
            {
                FindObjectOfType<MyPlaceView>().RefreshMySpaceItems();

                Refresh();
                _instantiatedPictureUiItem.Select(true);
                HidePanel();
            }
            else Debug.LogError("Door image doesn't change to empty");
        }
    }
}
