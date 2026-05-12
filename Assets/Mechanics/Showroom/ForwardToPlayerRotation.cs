using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForwardToPlayerRotation : MonoBehaviour
{
    private Camera _camera;
    
    void FixedUpdate()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_camera == null) return;

        Vector3 newForward = transform.position - _camera.transform.position;
        newForward.y = 0;

        if (newForward == Vector3.zero || Vector3.Dot(newForward, _camera.transform.forward) < 0)
        {
            return;
        }

        transform.right = -newForward;
    }
}