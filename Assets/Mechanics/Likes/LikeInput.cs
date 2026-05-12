using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeInput : MonoBehaviour
{
    [SerializeField] private Renderer _likeGreen;
    [SerializeField] private Renderer _dislikeRed;
    [SerializeField] private DoorPlayerSensor _sensor;
    [SerializeField] private GameObject _likesSwitcher;

    public void SetLikePressed(bool isPressed)
    {
        var color = _likeGreen.material.color;
        if (isPressed)
        {
            _likeGreen.gameObject.SetActive(true);
            _likeGreen.material.color = new Color(color.r, color.g, color.b, 1);
            //_likeGreen.material.SetFloat("_Alpha", 1f);
        }
        else
        {
            _likeGreen.gameObject.SetActive(false);
            _likeGreen.material.color = new Color(color.r, color.g, color.b, 0.35f);
            //_likeGreen.material.SetFloat("_Alpha", 0.15f);
        }        
    }

    public void SetDisLikePressed(bool isPressed)
    {
        var color = _dislikeRed.material.color;
        if (isPressed)
        {
            _dislikeRed.gameObject.SetActive(true);
            _dislikeRed.material.color = new Color(color.r, color.g, color.b, 1);
            //_dislikeRed.material.SetFloat("_Alpha", 1f);
        }
        else
        {
            _dislikeRed.gameObject.SetActive(false);
            _dislikeRed.material.color = new Color(color.r, color.g, color.b, 0.35f);
            //_dislikeRed.material.SetFloat("_Alpha", 0.15f);
        }
    }

    private void OnEnable()
    {
        _sensor.OnDoorNearEnter += _sensor_OnDoorNearEnter;
        _sensor.OnDoorNearExit += _sensor_OnDoorNearExit;
    }

    private void OnDisable()
    {
        _sensor.OnDoorNearEnter -= _sensor_OnDoorNearEnter;
        _sensor.OnDoorNearExit -= _sensor_OnDoorNearExit;
    }

    private void _sensor_OnDoorNearExit(Collider obj)
    {
        if (obj.GetComponentInParent<Camera>())
        {
            _likesSwitcher.SetActive(false);
        }
    }

    private void _sensor_OnDoorNearEnter(Collider obj)
    {
        if (obj.GetComponentInParent<Camera>())
        {
            _likesSwitcher.SetActive(true);
        }
    }
}
