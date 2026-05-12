using System.Collections;
using System.Collections.Generic;
using Assets.Mechanics.NetworkInteraction;
using Mechanics.LoadSceneObjects;
using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Mechanics.CustomMehanics
{
    public class BallonCroud : MonoBehaviour, INetworkCustomLogicService
    {
        #region Inspector

        [SerializeField] private Rigidbody _baloonParent;
        [SerializeField] private float _minDurationValue;
        [SerializeField] private float _maxDurationValue;
        [SerializeField] private float _minForseValue;
        [SerializeField] private float _maxForseValue;

        [SerializeField] private NetworkXrGrab _networkXrGrab;
        [SerializeField] private List<Rigidbody> _ballonRigidbodies;
        [SerializeField] private List<Material> _baloonCustomMaterials;

        #endregion

        private Coroutine _ballonRoutine;
        private StaticObject _staticObject;
        private CustomBehaviourNetworkObject _customBehaviourNetworkObject;

        private bool _isFlying = false;

        private Dictionary<Rigidbody, (Vector3, Vector3)> _startTransformValue = new();

        private void OnEnable()
        {
            _networkXrGrab.StartGrab += OnStartGrabbing;
            _networkXrGrab.StopGrab += OnStopGrabbing;
            _networkXrGrab.CurrentUserDrop += OnStopGrabbing;

            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnDisable()
        {
            _networkXrGrab.StartGrab -= OnStartGrabbing;
            _networkXrGrab.StopGrab -= OnStopGrabbing;
            _networkXrGrab.CurrentUserDrop -= OnStopGrabbing;

            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
            }
        }
//Baloons arch
        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            foreach (Rigidbody ballonRigidbody in _ballonRigidbodies)
            {
                _startTransformValue.Add(ballonRigidbody,
                    (ballonRigidbody.transform.localPosition, ballonRigidbody.transform.localRotation.eulerAngles));
                if (staticObject.PlaceNumber >= 10)
                {
                    if (ballonRigidbody.TryGetComponent(out MeshRenderer meshRenderer))
                    {
                        int materialsNumber = Random.Range(0,_baloonCustomMaterials.Count);
                        meshRenderer.material = _baloonCustomMaterials[materialsNumber];
                    }
                }
            }

            _customBehaviourNetworkObject = customBehaviourNetworkObject;
            _staticObject = staticObject;
            _customBehaviourNetworkObject.RequestStateFromServer();
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnStateChanged()
        {
            string currentState = _customBehaviourNetworkObject.GetState();
            bool newFlyingState;

            try
            {
                newFlyingState = JsonConvert.DeserializeObject<bool>(currentState);
            }
            catch (System.Exception)
            {
                Debug.Log($"no state for {gameObject.name}");
                return;
            }

            if (_isFlying == newFlyingState)
            {
                return;
            }

            _isFlying = newFlyingState;
            if (_isFlying)
            {
                AddForceOnBallons();
            }
            else
            {
                StartState();
            }
        }

        private void StartState()
        {
            foreach (Rigidbody ballonRigidbody in _ballonRigidbodies)
            {
                ballonRigidbody.transform.SetParent(_baloonParent.transform);

                ballonRigidbody.isKinematic = true;
                ballonRigidbody.transform.localPosition = _startTransformValue[ballonRigidbody].Item1;
                ballonRigidbody.transform.localRotation = Quaternion.Euler(_startTransformValue[ballonRigidbody].Item2);
            }
        }

        private void AddForceOnBallons()
        {
            _baloonParent.isKinematic = true;
            foreach (var ballonRigidbody in _ballonRigidbodies)
            {
                ballonRigidbody.transform.SetParent(null);
                ballonRigidbody.isKinematic = true;
                ballonRigidbody.velocity = Vector3.zero;
                ballonRigidbody.useGravity = false;
                ballonRigidbody.isKinematic = false;
                Vector3 XDirection = Random.Range(0, 100) % 2 == 0f ? Vector3.back : Vector3.forward;
                Vector3 ZDirection = Random.Range(0, 100) % 2 == 0f ? Vector3.right : Vector3.left;
                ballonRigidbody.AddForce((XDirection * Random.Range(_minForseValue, _maxForseValue)) / 4,
                    ForceMode.Impulse);
                ballonRigidbody.AddForce((ZDirection * Random.Range(_minForseValue, _maxForseValue)) / 4,
                    ForceMode.Impulse);

                ballonRigidbody.AddForce(Vector3.up * Random.Range(_minForseValue, _maxForseValue), ForceMode.Impulse);
            }
        }

        private void OnStopGrabbing()
        {
            StopCoroutine();
            if (_networkXrGrab._currentNetworkObject.IsOwner)
                _ballonRoutine = StartCoroutine(BallonRoutine());
        }

        private void OnStartGrabbing()
        {
            string jsonString = JsonConvert.SerializeObject(false);
            _customBehaviourNetworkObject.SetState(jsonString);
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
            if (_networkXrGrab._currentNetworkObject.IsOwner)
            {
                if (_customBehaviourNetworkObject != null)
                {
                    string jsonString = JsonConvert.SerializeObject(true);
                    _customBehaviourNetworkObject.SetState(jsonString);
                }
                else
                {
                    AddForceOnBallons();
                }
            }
            else
            {
                StopCoroutine();
            }

            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_minDurationValue, _maxDurationValue));
                foreach (var ballonRigidbody in _ballonRigidbodies)
                {
                    ballonRigidbody.isKinematic = true;
                }

                StopCoroutine();
            }
        }
    }
}