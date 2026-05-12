using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interactable;
using Assets.Scripts.Office;
using Cysharp.Threading.Tasks;
using Mechanics.SpaceStaticObjectEditing.Model;
using Meta.WitAi;
using Unity.VisualScripting;
using UnityEngine;

public class PictureImageSpaceEditingMode : MonoBehaviour
{
    [SerializeField] private SpaceStaticObjectModel _model;
    [SerializeField] private PicturePanel _panel;
    [SerializeField] private TriggerHandGrabbable _grab;
    private Transform _grabTransform;
    [SerializeField] private RectTransform _numberTransform;

    public event Action OnCenterGrabExit; 
    private Transform _cornersParent;
    private bool _blockCheckScale = false;

    private void OnEnable()
    {
        _grabTransform = _grab.transform;
        SetImage();
    }

    private void FixedUpdate()
    {
        CheckScale();
    }

    private void CheckScale()
    {
        if (!_blockCheckScale)
        {
            _grabTransform.localScale = new Vector3(
                1 / transform.localScale.x,
                1 / transform.localScale.y,
                1 / transform.localScale.z);
            _numberTransform.localScale = new Vector3(
                0.001f / transform.localScale.z,
                0.001f / transform.localScale.y,
                _numberTransform.localScale.z);
        }
    }

    private async void SetImage()
    {
        await UniTask.WaitUntil(() => _model.StaticObject != null);

        var pictureSpawner = FindObjectOfType<PicturesSpawnerService>();
        if (pictureSpawner != null && _model.StaticObject.PlaceNumber != null)
        {
            pictureSpawner.SetPictureFromEditingModeByNumber(_panel, _model.StaticObject.PlaceNumber.Value);
            
            if (_panel == null) return;
            _panel.SetAndShowNumber(_model.StaticObject.PlaceNumber.Value);
        }

        await UniTask.WaitUntil(() => _model.TriggerGrabInteractable != null);
        await UniTask.WaitUntil(() => _model.RightTriggerGrabInteractable != null);

        _cornersParent = _model.TriggerGrabInteractable.gameObject.transform.parent;

        _grab.gameObject.SetActive(true);
        _grabTransform.gameObject.SetActive(true);
        _grab.GrabStarted += CenterGrabStarted;
        _grab.GrabStopped += CenterGrabEnded;
    }

    private void CenterGrabStarted()
    {
        _blockCheckScale = true;
       _grab.transform.SetParent(transform.root);

        _model.TriggerGrabInteractable.gameObject.transform.SetParent(_grab.transform);
        _model.TriggerGrabInteractable.gameObject.transform.localScale = Vector3.zero;
        _model.RightTriggerGrabInteractable.gameObject.transform.SetParent(_grab.transform);
        _model.RightTriggerGrabInteractable.gameObject.transform.localScale = Vector3.zero;
    }

    private void CenterGrabEnded()
    {
        _blockCheckScale = false;
        _grab.transform.SetParent(transform);

        _model.TriggerGrabInteractable.gameObject.transform.SetParent(_cornersParent);
        _model.RightTriggerGrabInteractable.gameObject.transform.SetParent(_cornersParent);
        _model.TriggerGrabInteractable.gameObject.transform.localScale = Vector3.one * 0.1f;
        _model.RightTriggerGrabInteractable.gameObject.transform.localScale = Vector3.one * 0.1f;

        _grab.transform.localRotation = Quaternion.identity;
        _grab.transform.localPosition = Vector3.zero;
        _grab.transform.localScale = new Vector3(0.1f, 0.3f, 0.3f);

        OnCenterGrabExit?.Invoke();
    }

    private void OnDisable()
    {
        _grab.GrabStarted -= CenterGrabStarted;
        _grab.GrabStopped -= CenterGrabEnded;
    }
}
