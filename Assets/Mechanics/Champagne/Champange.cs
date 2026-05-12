using System.Collections;
using Assets.Mechanics.NetworkInteraction;
using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Champagne
{
    public class Champange : NetworkBehaviour
    {
        [SerializeField] private Rigidbody _currentRigidBody;
        [SerializeField] private Rigidbody _cork;
        [SerializeField] private Collider _corkCollider;
        [SerializeField] private ParticleSystem _spray;
        [SerializeField] private ParticleSystem _water;
        [SerializeField] private ParticleSystem _corkTail;
        [SerializeField] private NetworkXrGrab _networkXrGrab;
        [SerializeField] private float _movementThreshold = 0.2f;
        [SerializeField] private float _directionThreshold = 0.9f;
        [SerializeField] private float _corkMaxShift = 0.02f;
        [SerializeField] private AudioSource _openingAudioSource;
        [SerializeField] private AudioSource _pouringAudioSource;

        private Vector3 _corkStartPosition = Vector3.zero;
        private Vector3 _corkStartRotationEulerAngle = Vector3.zero;

        private Coroutine _checkCoroutine;
        private bool _isOpen = false;
        private Transform _corkParent;

        private void OnEnable()
        {
            _networkXrGrab.StartGrab += OnStartGrab;
            //_networkXrGrab.StopGrab += OnStopGrab;
            _networkXrGrab.CurrentUserDrop += OnStopGrab;

            _corkParent = _cork.transform.parent;
            _corkStartPosition = _cork.transform.localPosition;
            _corkStartRotationEulerAngle = _cork.transform.localRotation.eulerAngles;

            SetToIdleState();
        }

        private void OnDisable()
        {
            _networkXrGrab.StartGrab -= OnStartGrab;
            //_networkXrGrab.StopGrab -= OnStopGrab;
            _networkXrGrab.CurrentUserDrop -= OnStopGrab;
        }

        private void FixedUpdate()
        {
            if (!_isOpen) return;

            if (Vector3.Dot(transform.up, Vector3.up) < 0.1f)
            {
                if (!_water.isPlaying) _water.Play();
                if (!_pouringAudioSource.isPlaying) _pouringAudioSource.Play();
            }
            else
            {
                if (_water.isPlaying) _water.Stop();
                if (_pouringAudioSource.isPlaying) _pouringAudioSource.Stop();
            }
        }

        private void OnStartGrab() { if (IsOwner) StartChekingMovement(); }

        private void OnStopGrab() { if (IsOwner) SetIdleStateServerRpc(); }

        private void SetCorkStartPosition()
        {
            _corkCollider.isTrigger = true;
            _cork.isKinematic = true;
            _cork.transform.SetParent(_corkParent);
            _cork.transform.localPosition = _corkStartPosition;
            _cork.transform.localRotation = Quaternion.Euler(_corkStartRotationEulerAngle);
        }

        private void StartChekingMovement()
        {
            if (!IsOwner) return;

            //_currentRigidBody.isKinematic = false;
            if (_checkCoroutine == null) _checkCoroutine = StartCoroutine(CheckSpray());
        }

        private void SetToIdleState()
        {
            if (_checkCoroutine != null)
            {
                StopCoroutine(_checkCoroutine);
                _checkCoroutine = null;
            }

            _water.Stop();
            _spray.Stop();
            
            _pouringAudioSource.Stop();
            _openingAudioSource.Stop();
            
            SetCorkStartPosition();
            _isOpen = false;
        }

        private void StartCorkActivityAndSound()
        {
           _isOpen = true;

           if (_checkCoroutine != null)
           {
               StopCoroutine(_checkCoroutine);
               _checkCoroutine = null;
           }

           _cork.transform.SetParent(null);
           _cork.isKinematic = false;
           _corkCollider.isTrigger = false;

           _cork.AddForce(_cork.transform.up * 3, ForceMode.Impulse);
           if (!_corkTail.isPlaying) _corkTail.Play();
           if (!_spray.isPlaying) _spray.Play(); 
           if (!_openingAudioSource.isPlaying) _openingAudioSource.Play();
        }

        private IEnumerator CheckSpray()
        {
            Vector3 previousPosition = transform.position;
            Vector3 previousDirection = transform.up;
            const int maxShakeCount = 10;
            float corkShiftByOneShake = _corkMaxShift / maxShakeCount;
            int shakeCount = 0;
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (!_networkXrGrab.isSelected) continue;

                Vector3 currentPosition = transform.position;
                Vector3 movementVector = currentPosition - previousPosition;
                float movementMagnitude = movementVector.magnitude;

                Vector3 currentDirection = transform.up;
                float directionChangeMagnitude = Vector3.Cross(previousDirection, currentDirection).magnitude;
                if (movementMagnitude > _movementThreshold || directionChangeMagnitude > _directionThreshold)
                {
                    shakeCount++;
                    _cork.transform.position += corkShiftByOneShake * transform.up;

                    if (shakeCount > maxShakeCount) StartCorkActivityAndSoundServerRpc(); 
                }
                previousPosition = currentPosition;
                previousDirection = currentDirection;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartCorkActivityAndSoundServerRpc()
        {
            StartCorkActivityAndSoundClientRpc();
        }

        [ClientRpc]
        private void StartCorkActivityAndSoundClientRpc()
        {
            StartCorkActivityAndSound();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetIdleStateServerRpc()
        {
            SetIdleStateClientRpc();
        }

        [ClientRpc]
        private void SetIdleStateClientRpc()
        {
            SetToIdleState();
        }
    }
}