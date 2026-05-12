using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float _pollingTime = 1f;
    private float _time;
    private int _frameCount;

    private void Update()
    {
        _time += Time.deltaTime;

        _frameCount++;

        if (_time >= _pollingTime)
        {
            ConsoleController.frameRate = Mathf.RoundToInt(_frameCount / _time);

            _time -= _pollingTime;
            _frameCount = 0;
        }
    }
}
