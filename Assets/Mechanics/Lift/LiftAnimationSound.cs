using Oculus.Avatar2;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Mechanics.Lift
{
    public class LiftAnimationSound : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        [Space]
        [SerializeField] private DoorPlayerSensor _sensor;
        [SerializeField] private LiftController _controller;
        [SerializeField] private AudioSource _doorSound;
        [SerializeField] private AudioSource _buttonSoundSuccessful;
        [SerializeField] private AudioSource _buttonSoundUnsuccessful;
        [SerializeField] private AudioSource _liftFinishedSound;

        private bool _isOpened;

        private void Start()
        {
            _sensor.OnDoorNearEnter += OnDoorNearEnter;
            _sensor.OnDoorNearExit += OnDoorNearExit;
            _controller.ButtonSuccessfulClick += ButtonSuccessfulСlick;
            _controller.ButtonUnSuccessfulClick += ButtonUnsuccessfulСlick;
            _controller.LiftFinished += LiftFinished;

            if (PlayerLiftPosition.PutPlayerToTheLift) _liftFinishedSound.Play();
        }

        private void LiftFinished()
        {
            _liftFinishedSound.Play();
        }

        private void ButtonUnsuccessfulСlick()
        {
            if (_buttonSoundUnsuccessful.isPlaying) _buttonSoundUnsuccessful.Stop();
            _buttonSoundUnsuccessful.Play();
        }

        private void ButtonSuccessfulСlick()
        {
            if (_buttonSoundSuccessful.isPlaying) _buttonSoundUnsuccessful.Stop();
            _buttonSoundSuccessful.Play();
        }

        private void OnDestroy()
        {
            _sensor.OnDoorNearEnter -= OnDoorNearEnter;
            _sensor.OnDoorNearExit -= OnDoorNearExit;
            _controller.ButtonSuccessfulClick -= ButtonSuccessfulСlick;
            _controller.ButtonUnSuccessfulClick -= ButtonUnsuccessfulСlick;
            _controller.LiftFinished -= LiftFinished;
        }

        private void OnDoorNearEnter(Collider obj)
        {
            if (obj.GetComponent<OvrAvatarEntity>()) //doors open for all avatars
            {
                OpenDoors();
            }
        }

        private void OnDoorNearExit(Collider obj)
        {
            if (obj.GetComponentInParent<Camera>()) //doors close only when I leave the lift
            {
                CloseDoors();
            }
        }

        private void OpenDoors()
        {
            if (!_isOpened)
            {
                _animation.Play("LiftDoorsOpen");
                _doorSound.Play();
                _isOpened = true;
            }
        }

        private void CloseDoors()
        {
            if (_isOpened)
            {
                _animation.Play("LiftDoorsClose");
                _doorSound.Play();
                _isOpened = false;
            }
        }
    }
}
