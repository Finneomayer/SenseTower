using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropReturningObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _body;
    private Vector3 _starPosition;
    private Quaternion _startRotation;

    void Start()
    {
        _starPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            
            transform.position = _starPosition;
            transform.rotation = _startRotation;
            if (_body != null) _body.velocity = Vector3.zero;

        }
    } 
}
