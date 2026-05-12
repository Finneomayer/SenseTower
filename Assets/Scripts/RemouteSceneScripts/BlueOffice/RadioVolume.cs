using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioVolume : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audio;
    [SerializeField]
    private Transform _button;

    [SerializeField]
    private float _minPos = 0;
    [SerializeField]
    private float _maxPos = 0.067f;
    [SerializeField]
    private float _start = 0.4f;

    private Vector3 _startPos;
    private float _currentPosValue;

    private void Start()
    {
        _currentPosValue = _maxPos * _start;
        _startPos = _button.localPosition = new Vector3(_currentPosValue, _button.localPosition.y, _button.localPosition.z);
        _audio.volume = CalculateVolume(_maxPos * _start);
    }


    private void Update()
    {
        _currentPosValue = ClampPos(_button.localPosition.x);
        _audio.volume = CalculateVolume(_currentPosValue) / 8;
        _startPos.x = _currentPosValue;
        _button.transform.localPosition = _startPos;
    }

    private float CalculateVolume(float currentPos)
    {
        currentPos = ClampPos(currentPos);
        return Mathf.Abs(currentPos) / Mathf.Abs(_maxPos - _minPos);
    }

    private float ClampPos(float currentPos)
    {
        return Mathf.Clamp(_button.localPosition.x,
          Mathf.Min(_minPos, _maxPos),
         Mathf.Max(_minPos, _maxPos));
    }
}
