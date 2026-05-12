using Assets.Mechanics.Network.Scripts;
using SenseAI.Robot.UI;
using System;
using System.Collections;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SenseAI.Robot.Scripts
{
    public class NetworkRobot : NetworkBehaviour
    {
        #region Const

        private const float WaitingForUserInteractionSeconds = 15;

        #endregion

        #region Inspector

        [SerializeField]
        private XRBaseInteractable XRInteracatable;
        [SerializeField]
        private RobotDevice RobotDevice;
        [SerializeField]
        private RobotTransform RobotTransform;
        [SerializeField]
        private RobotInteractorReleationController RobotInteractionController;
        [SerializeField]
        private MVPNavigation RobotMVPNavigation;
        [SerializeField]
        private GameObject InteractionPanel;
        [SerializeField]
        private InfoPanelController InfoPanelController;
        [SerializeField]
        private Transform InfoPanelParent;
        [SerializeField] 
        private TrackedDeviceGraphicRaycaster _graphicRaycaster;
        [SerializeField]
        private Collider _colliderFilter;
        #endregion

        #region PrivateVariables

        private ulong _ownerClientId = 0;
        private ClientRpcParams _ownerClientRpcParams;
        private bool _isBusy;

        private Transform _currentInteractorTransform;
        private Coroutine _clientRefreshingInteractorPositionRoutine;
        private Coroutine _serverWaitingForUserInteractionRoutine;
        private Vector3 _currentInteractorPositionOnServer;

        #endregion

        #region UnityMethods

        private void OnEnable()
        {
            // Client events
            XRInteracatable.firstSelectEntered.AddListener(OnXRIntaractableSelectEnter);
            InfoPanelController.GoToOfficeButtonPressed += OnGoToOfficeRequested;
            InfoPanelController.GoToBaseButtonPressed += OnGoToBaseRequested;

            // Server events
            RobotMVPNavigation.GoToBaseStarted += OnServerGoToBaseStarted;
            RobotMVPNavigation.GoToOfficeFinished += OnServerGoToOfficeFinished;
            RobotDevice.PathCalculated += OnServerPathCalculated;
        }

        private void OnDisable()
        {
            // Client events
            XRInteracatable.firstSelectEntered.RemoveListener(OnXRIntaractableSelectEnter);
            InfoPanelController.GoToOfficeButtonPressed -= OnGoToOfficeRequested;
            InfoPanelController.GoToBaseButtonPressed -= OnGoToBaseRequested;

            // Server events
            RobotMVPNavigation.GoToBaseStarted -= OnServerGoToBaseStarted;
            RobotMVPNavigation.GoToOfficeFinished -= OnServerGoToOfficeFinished;
            RobotDevice.PathCalculated -= OnServerPathCalculated;
        }

        private void LateUpdate()
        {
            if (!IsClient)
            {
                return;
            }

            if (!InteractionPanel.activeInHierarchy)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 targetPosition = mainCamera.transform.position;
                targetPosition.y = InfoPanelParent.transform.position.y;
                InfoPanelParent.transform.LookAt(targetPosition);
            }
        }

        #endregion

        #region Network

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnNetworkManagerClientDisconnectCallback;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnNetworkManagerClientDisconnectCallback;
            }
            base.OnNetworkDespawn();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestInteractionServerRpc(ulong clientId, Vector3 interactorPosition)
        {
            if (_isBusy && clientId != _ownerClientId)
            {
                return;
            }

            RobotInteractionController.StopEscortToTarget();
            RobotDevice.CancelMoveToDestination();
            RobotMVPNavigation.InterupRobotOnWay();
            RobotTransform.LookAtPosition(interactorPosition);

            _isBusy = true;
            _ownerClientId = clientId;
            _ownerClientRpcParams = new()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            AllowInteractionClientRpc(_ownerClientRpcParams);

            ServerStartWaitingForUserInteraction();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestGoToOfficeServerRpc(ulong clientId, Vector3 interactorPosition)
        {
            if (clientId != _ownerClientId)
            {
                Debug.LogWarning("User is not owner of robot but is trying to manipulate it");
                return;
            }
            SendGoToOfficeStartedClientRpc();

            RobotInteractionController.StartEscortToTarget(interactorPosition);
            RobotMVPNavigation.GoToOffice();

            ServerStopWaitingForUserInteraction();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestGoToBaseServerRpc(ulong clientId)
        {
            if (clientId != _ownerClientId)
            {
                Debug.LogWarning("User is not owner of robot but is trying to manipulate it");
                return;
            }
            ServerSetFree();
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable, RequireOwnership = false)]
        private void SendInteractorPositionServerRpc(ulong clientId, Vector3 newPosition)
        {
            if (_ownerClientId == clientId)
            {
                _currentInteractorPositionOnServer = newPosition;
                RobotInteractionController.RefreshInteractorPosition(newPosition);
            }
        }

        [ClientRpc]
        private void AllowInteractionClientRpc(ClientRpcParams clientRpcParams = default)
        {
            InteractionPanel.SetActive(true);
            InfoPanelController.ShowFirstInteractMenuInvoke();
        }

        [ClientRpc]
        private void SendGoToOfficeStartedClientRpc(ClientRpcParams clientRpcParams = default)
        {
            InfoPanelController.ShowGoToOfficeStartMessage();
        }

        [ClientRpc]
        private void SendGoToOfficeFinishedClientRpc(ClientRpcParams clientRpcParams = default)
        {
            _graphicRaycaster.enabled = false;
            _colliderFilter.enabled = false;
            XRInteracatable.enabled = false;
            InfoPanelController.ShowGoToOfficeFinishedMessage();
            StopRefreshingInteractorPosition();
            RobotDevice.ClearPath();
        }

        [ClientRpc]
        private void SendGoToBaseStartedClientRpc(ClientRpcParams clientRpcParams = default)
        {
            _graphicRaycaster.enabled = true;
            _colliderFilter.enabled = true;
            XRInteracatable.enabled = true;
            InfoPanelController.HideEveryrhing();
            StopRefreshingInteractorPosition();
            RobotDevice.ClearPath();
        }

        [ClientRpc]
        private void SendPathCalculatedClientRpc(Vector3[] path, ClientRpcParams clientRpcParams = default)
        {
            RobotDevice.DrawPath(path);
        }

        #endregion

        #region Callbacks

        private void OnNetworkManagerClientDisconnectCallback(ulong clientId)
        {
            if (clientId == _ownerClientId)
            {
                ServerSetFree();
            }
        }

        private void OnXRIntaractableSelectEnter(SelectEnterEventArgs args)
        {
            StartInteraction(args);
        }

        private void OnGoToOfficeRequested()
        {
            RequestGoToOfficeServerRpc(NetworkManager.LocalClientId, _currentInteractorTransform.position);
            StartRefreshingInteractorPosition();
        }

        private void OnGoToBaseRequested()
        {
            if (NetworkManager != null)
            {
                RequestGoToBaseServerRpc(NetworkManager.LocalClientId);
            }
            else
            {
                RobotDevice.ReturnToBase();
            }
        }

        private void OnServerGoToBaseStarted()
        {
            _isBusy = false;
            RobotInteractionController.StopEscortToTarget();
            SendGoToBaseStartedClientRpc();
        }

        private void OnServerGoToOfficeFinished()
        {
            RobotInteractionController.StopEscortToTarget();
            RobotTransform.LookAtPosition(_currentInteractorPositionOnServer);
            SendGoToOfficeFinishedClientRpc();
        }

        private void OnServerPathCalculated(Vector3[] path)
        {
            SendPathCalculatedClientRpc(path, _ownerClientRpcParams);
        }

        #endregion

        #region PrivateMethods
        
        private void ServerSetFree()
        {
            _isBusy = false;
            ServerStopWaitingForUserInteraction();
            RobotInteractionController.StopEscortToTarget();
            RobotDevice.CancelMoveToDestination();
            RobotDevice.ReturnToBase();
        }

        private void StartInteraction(SelectEnterEventArgs args)
        {
            _currentInteractorTransform = args.interactorObject.transform;
            if (NetworkManager != null)
            {
                RequestInteractionServerRpc(NetworkManager.LocalClientId, _currentInteractorTransform.position);
            }
            else
            {
                AllowInteractionClientRpc();
            }
        }

        private void StartCoroutineWithRef(ref Coroutine coroutine, IEnumerator routine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(routine);
        }

        private void StopCoroutineWithRef(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private void StartRefreshingInteractorPosition()
        {
            StartCoroutineWithRef(ref _clientRefreshingInteractorPositionRoutine, ClientRefreshingInteractorPositionRoutine());
        }

        private void StopRefreshingInteractorPosition()
        {
            StopCoroutineWithRef(ref _clientRefreshingInteractorPositionRoutine);
        }

        private void ServerStartWaitingForUserInteraction()
        {
            StartCoroutineWithRef(ref _serverWaitingForUserInteractionRoutine, ServerWaitingForUserInteractionRoutine());
        }

        private void ServerStopWaitingForUserInteraction()
        {
            StopCoroutineWithRef(ref _serverWaitingForUserInteractionRoutine);
        }

        private IEnumerator ClientRefreshingInteractorPositionRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                SendInteractorPositionServerRpc(NetworkManager.LocalClientId, _currentInteractorTransform.position);
            }
        }

        private IEnumerator ServerWaitingForUserInteractionRoutine()
        {
            yield return new WaitForSeconds(WaitingForUserInteractionSeconds);

            ServerSetFree();
            _serverWaitingForUserInteractionRoutine = null;
        }

        #endregion
    }
}