using System;
using System.Collections;
using Assets.Mechanics.NetworkInteraction.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.NetworkInteraction
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkXrGrab : XRGrabInteractable
    {
        public event Action StartGrab;  
        public event Action StopGrab;  
        public event Action CurrentUserDrop;  
        private IGrabInteraction _grabInteractionService;
        public NetworkObject _currentNetworkObject { private set; get; }

        private Rigidbody _rigidbody;
        private int _interactionLayerMask = 0;
        private Coroutine _coroutine;
        private bool _startGravitySettings = false;
        private bool _startKinematicSettings = false;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();

            _startGravitySettings = _rigidbody.useGravity;
            _startKinematicSettings = _rigidbody.isKinematic;

            LoadSettings(false, true);
        }

        public void SetLocalUserOwnership()
        {
            if (_grabInteractionService != null)
            {
                _grabInteractionService.ChangeOwnership(_currentNetworkObject,
                    NetworkManager.Singleton.LocalClientId);
            }
        }

        public void Init(NetworkObject networkObject, IGrabInteraction grabInteractionService)
        {
            _currentNetworkObject = networkObject;
            _grabInteractionService = grabInteractionService;
            _interactionLayerMask = interactionLayers;

            if (transform.childCount > 0) //for nested ojects (chess for example)
            {
                var xrGrabs = GetComponentsInChildren<NetworkXrGrab>();

                foreach (var xrGrab in xrGrabs)
                {
                    if (xrGrab != null && xrGrab.gameObject.name != gameObject.name) 
                        xrGrab.Init(networkObject, grabInteractionService);
                }
            }

        }

        public void ForceDrop()
        {
            StopGrab?.Invoke();
            interactionLayers = 0;
            
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            _coroutine = StartCoroutine(ReturnInteract());
        }

        [ContextMenu("START GRAB")]
        protected override void Grab()
        {
            StartGrab?.Invoke();

            if (_grabInteractionService != null && _currentNetworkObject != null)
            {     
                _interactionLayerMask = interactionLayers;

                LoadSettings(_startGravitySettings, _startKinematicSettings);

                if (_currentNetworkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                {
                    _grabInteractionService.ChangeOwnership(_currentNetworkObject,
                        NetworkManager.Singleton.LocalClientId);
                }

                base.Grab();
            }
            else
            {
                base.Grab();
            }
        }

        [ContextMenu("DROP")]
        protected override void Drop()
        {
            base.Drop();
            CurrentUserDrop?.Invoke();
            Debug.LogWarning("*** Current user drop");
        }

        private void LoadSettings(bool useGravity, bool isKinematic)
        {
            _rigidbody.useGravity = useGravity;
            _rigidbody.isKinematic = isKinematic;
        }

        private IEnumerator ReturnInteract()
        {
            if (_currentNetworkObject != null && !_currentNetworkObject.IsOwnedByServer)
            {
                interactionLayers = _interactionLayerMask;
                LoadSettings(false, true);
            }

            yield return new WaitForSeconds(1f);
            interactionLayers = _interactionLayerMask;
            LoadSettings(false, true);
        }
    }
}