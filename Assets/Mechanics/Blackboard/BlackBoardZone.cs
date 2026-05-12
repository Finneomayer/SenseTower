using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Blackboard;
using Assets.Scripts.Player;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

public class BlackBoardZone : MonoBehaviour
{
    [SerializeField] private BlackBoardMarker _markerPrefab;
    [SerializeField] private BlackBoardEraser _eraserPrefab;
    [SerializeField] private BlackBoard _blackBoard;

    private BlackBoardEraser _blackBoardEraserInstance;
    private Coroutine _createMarkerRoutine;
    private PlayerController _controllerWithMarker;

    private Dictionary<ulong, BlackBoardMarker> _markers = new();

    public BlackBoard BlackBoard => _blackBoard;
    public BlackBoardMarker Marker => _markers[NetworkManager.Singleton.LocalClientId];
    public BlackBoardEraser Eraser => _blackBoardEraserInstance;

    public event Action<PlayerLogic> LocalPlayerEnteredZone;
    public event Action<PlayerLogic> LocalPlayerLeftZone;

    private void Awake()
    {
        if (_blackBoard is null)
            _blackBoard = FindObjectOfType<BlackBoard>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (_blackBoardMarkerInstance != null) return;

        var cam = other.GetComponent<SampleAvatarEntity>();
        if (cam == null) return;

        var player = cam.GetComponentInParent<PlayerLogic>();
        if (player == null)
            return;

        var ownerNetworkObject = player.GetComponent<NetworkPlayer>();
        //if (ownerNetworkObject != null && ownerNetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        //{
        
        if (NetworkManager.Singleton.LocalClientId == ownerNetworkObject.OwnerClientId)
            LocalPlayerEnteredZone?.Invoke(player);

        if (ownerNetworkObject.IsWinUser.Value) return; //disable marker for win user
        
        if (_createMarkerRoutine != null)
        {
            StopCoroutine(_createMarkerRoutine);
        }
        _createMarkerRoutine =
            StartCoroutine(CreateMarkerRoutine(player, ownerNetworkObject, ownerNetworkObject.OwnerClientId));
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        var cam = other.GetComponent<SampleAvatarEntity>();
        if (cam == null) return;

        var player = cam.GetComponentInParent<PlayerLogic>();
        if (player == null)
            return;
        var ownerNetworkObject = player.GetComponent<NetworkPlayer>();

        if (ownerNetworkObject.IsWinUser.Value == true) return;

        //if (ownerNetworkObject != null && ownerNetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        //{
        if (NetworkManager.Singleton.LocalClientId == ownerNetworkObject.OwnerClientId)
            LocalPlayerLeftZone?.Invoke(player);
        ProcessPlayerLeftZone(ownerNetworkObject.OwnerClientId);
        //}
    }

    private void OnDestroy()
    {
        ulong[] markerClientIds = _markers.Keys.ToArray();
        foreach (var clientId in markerClientIds)
        {
            ProcessPlayerLeftZone(clientId);
        }
    }

    public void SetActiveEraser(bool active)
    {
        SetActiveHandObject(_blackBoardEraserInstance, active);
        SetActiveHandObject(_markers[NetworkManager.Singleton.LocalClientId], !active);
    }

    private void SetActiveHandObject(MonoBehaviour handObject, bool active)
    {
        if (handObject != null)
        {
            handObject.gameObject.SetActive(active);
        }
    }

    private void ProcessPlayerLeftZone(ulong clientId)
    {
        if (_createMarkerRoutine != null)
        {
            StopCoroutine(_createMarkerRoutine);
            _createMarkerRoutine = null;
        }

        DestroyInstancedObjects(clientId);

        if (_controllerWithMarker != null)
        {
            _controllerWithMarker.SetActiveHaptics(false);
        }
    }

    private void DestroyInstancedObjects(ulong clientId)
    {
        if (_markers.TryGetValue(clientId, out var marker))
        {
            Destroy(marker.gameObject);
            _markers.Remove(clientId);
        }

        if (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClientId != clientId)
            return;

        if (_blackBoardEraserInstance != null)
            Destroy(_blackBoardEraserInstance.gameObject);
    }

    private IEnumerator CreateMarkerRoutine(PlayerLogic player, NetworkPlayer networkPlayer, ulong clientId)
    {
        yield return new WaitUntil(() => _blackBoard.IsInitialized);

        if (!_markers.ContainsKey(clientId))
        {
            PlayerController rightController = player.GetRightArm();

            BlackBoardMarker blackBoardMarkerInstance = Instantiate(_markerPrefab, player.transform);
            _markers[clientId] = blackBoardMarkerInstance;

            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                blackBoardMarkerInstance.Init(_blackBoard, rightController);
            }
            else
            {
                blackBoardMarkerInstance.transform.SetParent(networkPlayer.GetRemotePlayerRightHand());
                blackBoardMarkerInstance.transform.localPosition = new Vector3(-0.07223f,-0.07823f,-0.082f);
                blackBoardMarkerInstance.transform.localRotation = Quaternion.Euler(new Vector3(60f,-44f,-204f));
            }


            if (NetworkManager.Singleton.LocalClientId == clientId)
            {
                _blackBoardEraserInstance = Instantiate(_eraserPrefab, rightController.transform);

                _blackBoardEraserInstance.Init(_blackBoard, rightController.GrabInteractor);

                _controllerWithMarker = rightController;
                _controllerWithMarker.SetActiveHaptics(true);

                _createMarkerRoutine = null;
                _blackBoardEraserInstance.gameObject.SetActive(false);
            }
        }
    }
}