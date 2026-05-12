using Mechanics.LoadSceneObjects.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Mechanics.LoadSceneObjects
{
    public class CuttingRibbon : MonoBehaviour , INetworkCustomLogicService
    {
        [SerializeField]
        private GameObject[] ObjectsToOn;
        [SerializeField]
        private GameObject[] ObjectsToOff;
        [SerializeField]
        private AudioSource CuttingAudioSource;

        //[SerializeField]
        private CustomBehaviourNetworkObject _customBehaviourNetworkObject;

        private StaticObject _staticObject;
        private bool _initialStateSetted;
        private bool _isCutted;

        private void OnEnable()
        {
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;
            }
        }

        private void OnDisable()
        {
            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_customBehaviourNetworkObject == null)
            {
                return;
            }
            if (_isCutted)
            {
                return;
            }
            if (!other.TryGetComponent(out StaticObjectCollider staticObjectCollider)
                || staticObjectCollider.StaticObject == null)
            {
                return;
            }

            if (staticObjectCollider.StaticObject.ObjectKey == "Scissors")
            {
                SendState(true);
            }
        }

        public void Init(StaticObject staticObject, CustomBehaviourNetworkObject customBehaviourNetworkObject)
        {
            _staticObject = staticObject;
            _customBehaviourNetworkObject = customBehaviourNetworkObject;

            if (_customBehaviourNetworkObject != null)
            {
                _customBehaviourNetworkObject.StateChanged -= OnStateChanged;
                _customBehaviourNetworkObject.StateChanged += OnStateChanged;

                _customBehaviourNetworkObject.RequestStateFromServer();
            }
        }

        [ContextMenu("Cut in Editor")]
        private void CutInEditor()
        {
            SendState(true);
        }

        [ContextMenu("Restore in Editor")]
        private void RestoreInEditor()
        {
            SendState(false);
        }

        private void SendState(bool isCutted)
        {
            string jsonString = JsonConvert.SerializeObject(isCutted);
            _customBehaviourNetworkObject.SetState(jsonString);
        }

        private void SetInitialState(bool isCutted)
        {
            _isCutted = isCutted;
            SetActiveObjectsByState(_isCutted);
        }

        private void SetState(bool isCutted)
        {
            _isCutted = isCutted;
            SetActiveObjectsByState(_isCutted);

            if (_isCutted)
            {
                CuttingAudioSource.Play();
            }
        }

        private void SetActiveObjectsByState(bool isCutted)
        {
            foreach (var item in ObjectsToOn)
            {
                item.SetActive(isCutted);
            }
            foreach (var item in ObjectsToOff)
            {
                item.SetActive(!isCutted);
            }
        }

        private void OnStateChanged()
        {
            string currentState = _customBehaviourNetworkObject.GetState();

            bool isInitialState = !_initialStateSetted;
            _initialStateSetted = true;

            bool newCuttedState;
            try
            {
                newCuttedState = JsonConvert.DeserializeObject<bool>(currentState);
            }
            catch (System.Exception)
            {
                Debug.Log($"no state for {gameObject.name}");
                return;
            }

            if (_isCutted == newCuttedState)
            {
                return;
            }

            if (isInitialState)
            {
                SetInitialState(newCuttedState);
            }
            else
            {
                SetState(newCuttedState);
            }
        }

    }
}