using System;
using UnityEngine;

[Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        if (rigTarget == null || vrTarget == null) return;

        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRig : MonoBehaviour
{
    [SerializeField] private Transform _headConstraint;
    [SerializeField] private Vector3 _headBodyOffset;
    [SerializeField] private float _smoothness = 1;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;
    

    private void Start()
    {
        _headBodyOffset = transform.position - _headConstraint.position;
    }

    private void Update()
    {
        transform.position = _headConstraint.position + _headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(_headConstraint.forward, Vector3.up).normalized, Time.deltaTime * _smoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
