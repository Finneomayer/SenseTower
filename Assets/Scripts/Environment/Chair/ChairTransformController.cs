using System;
using Unity.Netcode;
using UnityEngine;

public class ChairTransformController : MonoBehaviour
{
    [SerializeField]private Transform _target;
    private Transform _playerParentTransform;
    private Vector3 _startEulerAngles;
    private float _originalXAngle;
    private float _originalZAngle;
    private Place _place;
    
    private void Start()
    {
        _place = GetComponentInParent<Place>();
        _startEulerAngles = transform.eulerAngles;
        _originalXAngle = transform.eulerAngles.x;
        _originalZAngle = transform.eulerAngles.z;
    }

    private void Update()
    {
        if (_target != null)
            BindChairRotation();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        
        if(target == null)
            UnBindChairRotation();
    }

    private void BindChairRotation()
    {
        //checking "_place != null" is for EnterScene where is no Place script, so place cannot be occupied
        if (_target == null || _place != null && _place.IsOccupiedID.Value == 0)
        {
            return;
        }
        var yAngle = _target.eulerAngles.y;
        transform.eulerAngles = new Vector3(_originalXAngle, yAngle, _originalZAngle);
    }

    public void UnBindChairRotation()
    {
        transform.eulerAngles = _startEulerAngles;
    }

    public void CenterPosition()
    {
        if (_playerParentTransform == null)
        {
            return;
        }
        
        _playerParentTransform.position =
            new Vector3(
                transform.position.x,
               _playerParentTransform.position.y,
                transform.position.z); 
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.GetComponent<Camera>())
        //{
        //    _target = other.gameObject.transform;
        //}
//
        //if (other.gameObject.GetComponent<Rigidbody>())
        //{
        //    _playerParentTransform = other.gameObject.transform;
        //}
        //
        // TODO: Remove commented code and methods connected with it.
        // This manual setting of player transform position and rotation was replaced by  
        // Teleportation Anchor standard setting Match Orientation setted to Target Up And Forward
        //LookForward();
        //CenterPosition();
    }

    //private void LookForward()
    //{
    //    if (_playerParentTransform != null)
    //        _playerParentTransform.eulerAngles = new Vector3(_playerParentTransform.eulerAngles.x, 0, _playerParentTransform.eulerAngles.z);
    //}

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.GetComponent<Camera>())
        //{
        //    _target = other.gameObject.transform;
        //}
//
        //if (other.gameObject.GetComponent<Rigidbody>())
        //{
        //    _playerParentTransform = other.gameObject.transform;
        //}
//
        //BindChairRotation(); 
    }

    private void OnTriggerExit(Collider other)
    {
        //_target = transform;
        //_playerParentTransform = null;
        //UnBindChairRotation();
    }
}
