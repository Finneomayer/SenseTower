using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.TextToSpeech;
using Assets.Scripts.Data;
using Assets.Scripts.Environmental;
using Assets.Scripts.Gallery;
using TMPro;
using UnityEngine;

public class PicturePanelInfo : MonoBehaviour
{
    [Header("Picture description")]
    [SerializeField] private RectTransform _descriptionPanel;
    [SerializeField] private TMP_Text _author;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private GameObject _separator2;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Vector3 _offset;
    [Space]
    [Header("Text to speech player")]
    [SerializeField] private SimpleTextToSpeechPlayer _textToSpeechPlayer;

    [Space]
    [Header("Trigger")]
    [SerializeField] private NetworkTriggerObserver _trigger;

    private Canvas _pictureDescriptionCanvas;

    private void Awake()
    {
        _pictureDescriptionCanvas = _descriptionPanel.GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        _trigger.LocalClientEnterTrigger += _trigger_LocalClientEnterTrigger;
        _trigger.LocalClientExitTrigger += _trigger_LocalClientExitTrigger;
    }

    private void _trigger_LocalClientExitTrigger()
    {
        _panel.SetActive(false);
    }

    private void _trigger_LocalClientEnterTrigger(GameObject obj)
    {
        _panel.SetActive(true);
    }

    private void OnDisable()
    {
        _trigger.LocalClientEnterTrigger -= _trigger_LocalClientEnterTrigger;
        _trigger.LocalClientExitTrigger -= _trigger_LocalClientExitTrigger;
    }

    public void SetDescription(TowerPicture towerPicture)
    {
        _descriptionPanel.localScale = new Vector3(
            _descriptionPanel.localScale.x / transform.localScale.z,
            _descriptionPanel.localScale.y / transform.localScale.y,
            _descriptionPanel.localScale.z / transform.localScale.x);
        
        Vector3 scaledPosition = transform.transform.localScale / 2;
        Vector3 resultPositionVector =
            transform.transform.position +
            (-transform.transform.forward * (scaledPosition.z + _offset.z))
            + (-transform.transform.right * _offset.x);
        _descriptionPanel.transform.position = resultPositionVector;
        
        _author.text = towerPicture.Author.StringConvertionToGalleryDescription();
        _name.text = towerPicture.Name.StringConvertionToGalleryDescription();
        _description.text = towerPicture.Description.StringConvertionToGalleryDescription();

        if (!String.IsNullOrEmpty(towerPicture.Author)
            || !String.IsNullOrEmpty(towerPicture.Name)
            || !String.IsNullOrEmpty(towerPicture.Description))
        {
            _pictureDescriptionCanvas.enabled = true;
        }
        else
        {
            _pictureDescriptionCanvas.enabled = false;
        }

        if (!String.IsNullOrEmpty(towerPicture.Author))
        {
            _author.gameObject.SetActive(true);

            if (!String.IsNullOrEmpty(towerPicture.Name))
            {
                _name.gameObject.SetActive(true);

                if (!String.IsNullOrEmpty(towerPicture.Description)) // 1 - 1 - 1 // 1 - 1 - 0
                {
                    _description.gameObject.SetActive(true);
                    _separator2.SetActive(true);
                }
            }
            else if (!String.IsNullOrEmpty(towerPicture.Description)) //1 - 0 - 1 //1 - 0 - 0
            {
                _description.gameObject.SetActive(true);
                _separator2.SetActive(true);
            }
        }
        else if (!String.IsNullOrEmpty(towerPicture.Name))
        {
            _name.gameObject.SetActive(true);

            if (!String.IsNullOrEmpty(towerPicture.Description)) //0 - 1 - 1 //0 - 1 - 0
            {
                _description.gameObject.SetActive(true);
                _separator2.SetActive(true);
            }
        }
        else if (!String.IsNullOrEmpty(towerPicture.Description)) //0 - 0 - 1 
        {
            _description.gameObject.SetActive(true);
        }
        else //0 - 0 - 0
        {
            //_descriptionPanel.gameObject.SetActive(false);
        }

        List<string> textBlocks = new();
        if (!string.IsNullOrEmpty(towerPicture.Author))
        {
            textBlocks.Add(towerPicture.Author);
        }
        if (!string.IsNullOrEmpty(towerPicture.Name))
        {
            textBlocks.Add(towerPicture.Name);
        }
        if (!string.IsNullOrEmpty(towerPicture.Description))
        {
            textBlocks.Add(towerPicture.Description);
        }

        _textToSpeechPlayer.Init(textBlocks);
    }
}
