using UnityEngine;
using UnityEngine.Events;

namespace Sense.RemouteScene
{
    public class PhysicsButton : MonoBehaviour
    {

        [SerializeField] private float _threshold = 0.1f;
        [SerializeField] private float _deadZone = 0.025f;

        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private bool _isPressed;
        private Vector3 _startPos;
        private ConfigurableJoint _joint;

        private void Start()
        {
            _startPos = transform.localPosition;
            _joint = GetComponent<ConfigurableJoint>();
        }


        private void Update()
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
            if (!_isPressed && GetValue() + _threshold >= 1) Pressed();
            if (_isPressed && GetValue() - _threshold <= 0) Released();
        }

        private float GetValue()
        {
            var value = Vector3.Distance(_startPos, transform.localPosition) * 10;


            if (Mathf.Abs(value) < _deadZone)
                value = 0;

            return Mathf.Clamp(value, -1, 1);
        }

        private void Pressed()
        {
            _isPressed = true;
            OnPressed?.Invoke();
        }

        private void Released()
        {
            _isPressed = false;
            OnReleased?.Invoke();
        }
    }
}