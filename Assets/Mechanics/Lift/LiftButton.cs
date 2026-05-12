using System;
using Assets.Mechanics.PadKeyboard;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Lift
{
    [Serializable]
    public struct LiftButtonTarget
    {
        public int HallIndex;
        public int FloorIndex;
        public Transform[] ExitPoints;
    }

    public class LiftButton : MonoBehaviour
    {
        public bool Blocked = true;
        public bool AlreadyOnThisFloor = false;
        [SerializeField] private LiftButtonTarget _target;
        [SerializeField] private FingerPhysicButton _physicButton;
        [SerializeField] private Button _raycastButton;
        [SerializeField, Space] private float _doubleClickDelay = 3f;

        public event Action<LiftButtonTarget> OnLiftButtonPressed;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        private float _clickTime = 0;

        private void OnEnable()
        {
            _physicButton.OnPress += PhysicButtonPressed;
            _physicButton.OnHoverEnter += PhysicButtonOnOnHoverEnter;
            _physicButton.OnHoverExit += PhysicButtonOnOnHoverExit;
        }

        private void PhysicButtonOnOnHoverEnter() => OnHoverEnter?.Invoke();
        private void PhysicButtonOnOnHoverExit() => OnHoverExit?.Invoke();

        private void OnDisable()
        {
            _physicButton.OnPress -= PhysicButtonPressed;
        }

        private void PhysicButtonPressed()
        {
            InvokePressEvent();
        }

        private void InvokePressEvent()
        {
            if (Time.time - _clickTime > _doubleClickDelay)
            {
                OnLiftButtonPressed?.Invoke(_target);
                _clickTime = Time.time;
            }
        }
    }
}
