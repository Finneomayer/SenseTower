using UnityEngine;
using UnityEngine.XR.Management;

namespace Assets.Scripts.Environmental
{
    public class PlayerPositionCorrection : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _camera;

        [SerializeField] [Range(1.3f, 2f)] private float _maxHeight = 1.55f;
        [SerializeField] [Range(0.5f, 1.2f)] private float _minHeight = 0.95f;

        private bool _isChecking = true;


        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_isChecking)
            {
                if (XRGeneralSettings.Instance.Manager.isInitializationComplete) CorrectPosition();
            }
#endif   
        }

        private void FixedUpdate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (((_camera.localPosition.y + _player.position.y) < _minHeight) || ((_camera.localPosition.y + _player.position.y) > _maxHeight))
                CorrectPositionY();
#endif
        }

        private void CorrectPosition()
        {
            _isChecking = false;

            var _delta = _camera.localPosition;
            _player.position = new Vector3(-_delta.x, - _delta.y + 1.25f, -_delta.z);
        }

        private void CorrectPositionY()
        {
            var _delta = _camera.localPosition;
            _player.position = new Vector3(_player.position.x, -_delta.y + 1.25f, _player.position.z);
        }
    }
}
