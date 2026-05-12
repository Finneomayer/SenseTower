using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.NetworkInteraction;
using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Unity.Networking.Transport;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Mechanics.CustomMehanics
{
    public class Ballon : MonoBehaviour, INetworkCustomLogicService
    {
        #region Inspector

        [SerializeField] private float _minDurationValue = 0.6f;
        [SerializeField] private float _maxDurationValue = 1.4f;
        [SerializeField] private float _minForseValue = 0.6f;
        [SerializeField] private float _maxForseValue = 1.5f;

        [SerializeField] private NetworkXrGrab _networkXrGrab;
        [SerializeField] private Rigidbody _ballonRigidbody;

        #endregion

        private Coroutine _ballonRoutine;
        private StaticObject _staticObject;
        private CustomBehaviourNetworkObject _customBehaviourNetworkObject;

        private void OnEnable()
        {
            _networkXrGrab = GetComponent<NetworkXrGrab>();
            _ballonRigidbody = GetComponent<Rigidbody>();

            _networkXrGrab.StartGrab += OnStartGrabbing;
            _networkXrGrab.StopGrab += OnStopGrabbing;
            _networkXrGrab.CurrentUserDrop += OnDrop;
        }

        private void OnDisable()
        {
            _networkXrGrab.StartGrab -= OnStartGrabbing;
            _networkXrGrab.StopGrab -= OnStopGrabbing;
            _networkXrGrab.CurrentUserDrop -= OnDrop;
        }

        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            _customBehaviourNetworkObject = customBehaviourNetworkObject;
            _staticObject = staticObject;
        }
        
        private void AddForceOnBallons()
        {
            _ballonRigidbody.isKinematic = true;
            _ballonRigidbody.velocity = Vector3.zero;
            _ballonRigidbody.useGravity = false;
            _ballonRigidbody.isKinematic = false;
            _ballonRigidbody.AddForce(Vector3.up * Random.Range(_minForseValue, _maxForseValue), ForceMode.Impulse);
        }

        private void OnStopGrabbing()
        {
            StopCoroutine();
            if (_networkXrGrab._currentNetworkObject.IsOwner)
                _ballonRoutine = StartCoroutine(BallonRoutine());
        }

        private void OnDrop()
        {
            StopCoroutine();
            _ballonRoutine = StartCoroutine(BallonRoutine());
        }

        private void OnStartGrabbing()
        {
            StopCoroutine();
        }

        private void StopCoroutine()
        {
            if (_ballonRoutine != null)
            {
                StopCoroutine(_ballonRoutine);
                _ballonRoutine = null;
            }
        }

        private IEnumerator BallonRoutine()
        {
            yield return new WaitForSeconds(0.1f);

            AddForceOnBallons();

            yield return new WaitForSeconds(Random.Range(_minDurationValue, _maxDurationValue));
            _ballonRigidbody.isKinematic = true;

            StopCoroutine();
        }
    }
}