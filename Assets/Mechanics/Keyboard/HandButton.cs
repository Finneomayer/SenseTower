using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HandButton : XRBaseInteractable
{    
    public event Action OnPressPhysicsButton = null;

    [SerializeField]private float _zMin = 0.0f;
    [SerializeField]private float _zMax = 0.0f;
    private bool _previousPress = false;

    private float _previousHandHeight = 0.0f;//
    private XRBaseInteractor _hoverInteractor = null;//

    protected override void Awake()//
    {
        base.Awake();//
        hoverEntered.AddListener(StartPress);//
        hoverExited.AddListener(EndPress);
    }

    private void Start()
    {
        SetMinMax();
    }

    private void StartPress(HoverEnterEventArgs args)//
    {
        _hoverInteractor = args.interactor;//
        _previousHandHeight = GetLocalZPosition(_hoverInteractor.transform.position);//change
        //previousHandHeight = args.interactor.transform.position.y;
    }

    private void EndPress(HoverExitEventArgs args) //
    {
        _hoverInteractor = null;//
        _previousHandHeight = 0.0f;//

        _previousPress = false;
        SetZPosition(_zMax);    
    }

    protected override void OnDestroy()//
    {
        base.OnDestroy();
        hoverEntered.RemoveListener(StartPress);
        hoverExited.RemoveListener(EndPress);
    }   

    private void SetMinMax()
    {
        Collider collider = GetComponent<Collider>();
        
        if(Math.Abs(_zMin) < 0.001f  && Math.Abs(_zMin) > -0.001f)
            _zMin = transform.localPosition.z - (collider.bounds.size.z * 0.3f / transform.parent.localScale.z);
       
        _zMax = transform.localPosition.z;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (_hoverInteractor)
        {
            float newHandHeight = GetLocalZPosition(_hoverInteractor.transform.position);
            float handDifference = _previousHandHeight - newHandHeight;
            _previousHandHeight = newHandHeight;

            float newPosition = transform.localPosition.z - handDifference;
            SetZPosition(newPosition);

            CheckPress();
        }
    }

    private float GetLocalZPosition(Vector3 position)
    {
        Vector3 localPosition = transform.parent.InverseTransformPoint(position);
        return localPosition.z;
    }

    private void SetZPosition(float position)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.z = Mathf.Clamp(position, _zMin, _zMax);
        transform.localPosition = newPosition;
    }

    private void CheckPress()
    {
        bool inPosition = InPosition();

        if (inPosition &&  inPosition != _previousPress)
        {
            OnPressPhysicsButton?.Invoke();
        }
        _previousPress = inPosition;

    }

    private bool InPosition()
    {
        float inRange = Mathf.Clamp(transform.localPosition.z, _zMin, _zMin + 0.01f * transform.parent.localScale.z);
        return transform.localPosition.z == inRange;
    }
}
