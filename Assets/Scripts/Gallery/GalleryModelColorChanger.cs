using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GalleryModelColorChanger : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private List<Color> _colors;
    [SerializeField] private GalleryLeavesCollider _collider;
    public bool isPulsating = false;
    private Color _startColor;
    private Color _targetColor;
    
    private int _targetColorNumber;
    private float _lastColorChangeTime;
    [SerializeField] [Range(0.1f, 2f)] private float _duration = 1f;

    private void Start()
    {
        _colors.Add(_renderer.material.color);  //RGBA(0.418, 0.896, 0.410, 1.000)
        _startColor = _colors[_colors.Count - 1];
        _targetColorNumber = 0;
        _targetColor = _colors[_targetColorNumber];

        _collider.OnTriggerStayAction += _collider_OnTriggerStayAction;
    }

    private void _collider_OnTriggerStayAction(bool isInTrigger)
    {
        if (isInTrigger)
        {
            isPulsating = true;
            _targetColorNumber = 0;
            _targetColor = _colors[_targetColorNumber];
            _startColor = _colors[_colors.Count - 1];
            _lastColorChangeTime = Time.time;
        }
        else
        {
            isPulsating = false;
            _lastColorChangeTime = Time.time;
            _startColor = _renderer.material.color;
        }
    }

    void Update()
    {
        
        if (!isPulsating)
        {
            _targetColor = _colors[_colors.Count - 1];
        }
        
        var ratio = (Time.time - _lastColorChangeTime) / _duration;
        ratio = Mathf.Clamp01(ratio);

        _renderer.material.color = Color.Lerp(_startColor, _targetColor, ratio);
        //material.color = Color.Lerp(startColor, endColor, Mathf.Sqrt(ratio)); // A cool effect
        //material.color = Color.Lerp(startColor, endColor, ratio * ratio); // Another cool effect

        if (ratio == 1f && isPulsating)
        {
            _lastColorChangeTime = Time.time;

            if (_targetColorNumber < _colors.Count - 1)
            {
                _startColor = _colors[_targetColorNumber];
                _targetColor = _colors[_targetColorNumber + 1];

                _targetColorNumber++;
            }
            else
            {
                _startColor = _colors[_targetColorNumber];
                _targetColor = _colors[0];

                _targetColorNumber = 0;
            }
        }
    }
}

