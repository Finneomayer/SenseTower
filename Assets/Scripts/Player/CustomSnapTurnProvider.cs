using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using InputDevice = UnityEngine.XR.InputDevice;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

public class CustomSnapTurnProvider : MonoBehaviour
{
    [SerializeField] private InputActionProperty _turnInput;
    [SerializeField] private float _turnAmount = 30f;
    [Space]
    [SerializeField] private Transform _playerPos;
    [SerializeField] private Transform _cameraPos;
    [SerializeField] private Transform _cameraCompensation;
    private InputDevice _inputSystem;
    private NetworkPlayer _player;

    private void Start()
    {
        _player = GetComponent<NetworkPlayer>();
        if (!_player.IsOwner) return;
        _turnInput.action.started += Action_started;
    }

    private void OnDestroy()
    {
        if (!_player.IsOwner) return;
        _turnInput.action.started -= Action_started;
    }

    private void Action_started(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        RotatePlayer(input.x > 0f ? 1 : -1);
    }

    private void RotatePlayer(int direction)
    {
        if (_cameraPos != null && _cameraCompensation != null && _playerPos != null)
        {
            _cameraCompensation.localPosition =
                new Vector3(-_cameraPos.localPosition.x, _cameraCompensation.localPosition.y, -_cameraPos.localPosition.z);
            
            _playerPos.RotateAround(_cameraPos.position, Vector3.up, _turnAmount * direction);
        }
    }

    public Vector3 GetShift()
    {
        return new Vector3(-_cameraPos.localPosition.x, _cameraPos.localPosition.y, -_cameraPos.localPosition.z);
    }
}
