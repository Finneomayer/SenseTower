using System;
using Assets.Mechanics.Chess.Scripts;
using Assets.Mechanics.NetworkInteraction;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ChessController : NetworkBehaviour
{
    public ulong GrabingOwnerIdServer;

    public NetworkVariable<int> WhiteEatenCount;
    public NetworkVariable<int> BlackEatenCount;

    [SerializeField] private NetworkXrGrab _xrGrab;
    [SerializeField] private Transform[] _whiteEatenPlaces;
    [SerializeField] private Transform[] _blackEatenPlaces;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider[] _boardColliders;

    private ChessPiece[] _chessPieces;
    private NetworkXrGrab[] _childXrGrabs;
    private Vector3[] _startPositions;
    private Quaternion[] _startRotations;
    private bool _isCheckingPositionForReset;
    private Coroutine _delayCoroutine;

    private void OnEnable()
    {
        _chessPieces = GetComponentsInChildren<ChessPiece>();
        _childXrGrabs = GetComponentsInChildren<NetworkXrGrab>();

        _xrGrab.StartGrab += OnGrabBoard;
        _xrGrab.CurrentUserDrop += OnDrop;
        //_xrGrab.StopGrab += OnDrop;

        foreach (var xrGrab in _childXrGrabs)
        {
            if (xrGrab.gameObject.name == gameObject.name) continue;

            xrGrab.StartGrab += OnGrabChildPiece;
            xrGrab.CurrentUserDrop += OnDrop;
            //xrGrab.StopGrab += OnDrop;
        }

        foreach (var piece in _chessPieces)
        {
            piece.IsEaten += PieceIsEaten;
        }

        SetStartPositions();
    }

    private void OnDisable()
    {
        _xrGrab.StartGrab -= OnGrabBoard;
        _xrGrab.CurrentUserDrop -= OnDrop;
        //_xrGrab.StopGrab -= OnDrop;

        foreach (var xrGrab in _childXrGrabs)
        {
            if (xrGrab.gameObject.name == gameObject.name) continue;

            xrGrab.StartGrab -= OnGrabChildPiece;
            xrGrab.CurrentUserDrop -= OnDrop;
            //xrGrab.StopGrab -= OnDrop;
        }

        foreach (var piece in _chessPieces)
        {
            piece.IsEaten -= PieceIsEaten;
        }
    }

    private void FixedUpdate()
    {
#if !UNITY_SERVER
        if (_isCheckingPositionForReset)
        {
            if ((transform.rotation.eulerAngles.x > 90 && transform.rotation.eulerAngles.x < 270) ||
                (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270))
            {
                ResetChess();
            }
        }
#endif
    }

    private IEnumerator DelayCoroutine(float delay)
    {
        if (IsOwner) ChildAndParentColliderSetEnable(true);
        yield return new WaitForSeconds(delay);
        ChildAndParentColliderSetEnable(true);
    }

    [ClientRpc]
    public void OwnerChangedClientRpc(ulong newId, bool isGrabbingBoard)
    {
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);

        _isCheckingPositionForReset = false;
        SetPiecesUserAsOwner(false);

        if (newId == NetworkManager.Singleton.LocalClientId)
        {
            ParentAsInSourcePrefab();
            ChildColliderSetEnable(false);
            ChildBodiesSetKinematic(true);

            if (isGrabbingBoard) _isCheckingPositionForReset = true;
            else SetPiecesUserAsOwner(true);
        }
        else if (newId == 0) //while nobody holding
        {
            _delayCoroutine = StartCoroutine(DelayCoroutine(1f));
            ChildAndParentXrGrabsSetEnable(true);
            //ChildBodiesSetKinematic(true);  is disabled to have throwing ability
        }
        else //while holding other user
        {
            if (isGrabbingBoard) ParentToBoardObject();
            else ParentAsInSourcePrefab();

            ChildAndParentXrGrabsSetEnable(false);
            ChildAndParentColliderSetEnable(false);
            ChildBodiesSetKinematic(true);
        }
    }
    
    private void OnGrabBoard() { ChangeGrabberServerRpc(NetworkManager.LocalClientId, true); }

    private void OnGrabChildPiece() { ChangeGrabberServerRpc(NetworkManager.LocalClientId, false); }

    private void OnDrop() { ChangeGrabberServerRpc(0); }


    [ServerRpc(RequireOwnership = false)]
    private void SetWhiteCountServerRpc(int count) { WhiteEatenCount.Value = count; }

    [ServerRpc(RequireOwnership = false)]
    private void SetBlackCountServerRpc(int count) { BlackEatenCount.Value = count; }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Unreliable)]
    private void ResetPiecesPositionsServerRpc() { ResetPiecesPositionsClientRpc(); }


    [ServerRpc(RequireOwnership = false)]
    private void ChangeGrabberServerRpc(ulong thisGrabbingId, bool isGrabbingBoard = false)
    {
        if (GrabingOwnerIdServer != 0 && thisGrabbingId != 0) return;
        GrabingOwnerIdServer = thisGrabbingId;
        OwnerChangedClientRpc(thisGrabbingId, isGrabbingBoard);
    }


    [ClientRpc]
    private void ResetPiecesPositionsClientRpc()
    {
        ParentAsInSourcePrefab();
        if (IsOwner)
        {
            for (int i = 0; i < _chessPieces.Length; i++)
            {
                _chessPieces[i].ResetMaterial();
                _chessPieces[i].ResetScaleAndLocalPosition();

                _chessPieces[i].transform.localPosition =
                    Vector3.MoveTowards(_chessPieces[i].transform.localPosition, _startPositions[i],
                        0.3f * (Vector3.Distance(_chessPieces[i].transform.localPosition, _startPositions[i])));
                _chessPieces[i].transform.localRotation = _startRotations[i];
            }
        }
    }

    private void ChildColliderSetEnable(bool enable)
    {
        if (_chessPieces == null || _chessPieces.Length == 0) return;
        foreach (var piece in _chessPieces)
        {
            piece.SetMeshColliderAvailable(enable);
        }
    }

    private void ChildAndParentColliderSetEnable(bool enable)
    {
        if (_chessPieces == null || _chessPieces.Length == 0) return;

        foreach (var collider in _boardColliders)
        {
            collider.enabled = enable;
        }
        foreach (var piece in _chessPieces)
        {
            piece.SetMeshColliderAvailable(enable);
        }
    }

    private void ChildAndParentXrGrabsSetEnable(bool enable)
    {
        _xrGrab.enabled = enable;

        foreach (var grab in _childXrGrabs)
        {
            grab.enabled = enable;
        }
    }

    private void ChildBodiesSetKinematic(bool isKinematic)
    {
        if (_chessPieces == null || _chessPieces.Length == 0) return;
        foreach (var piece in _chessPieces)
        {
            piece.SetKinematic(isKinematic);
            piece.SetUseGravity(!isKinematic);
        }
    }

    private void SetStartPositions()
    {
        _startPositions = new Vector3[_chessPieces.Length];
        _startRotations = new Quaternion[_chessPieces.Length];

        for (int i = 0; i < _chessPieces.Length; i++)
        {
            _startPositions[i] = _chessPieces[i].transform.localPosition;
            _startRotations[i] = _chessPieces[i].transform.localRotation;
        }
    }

    private void SetPiecesUserAsOwner(bool isOwner)
    {
        foreach (var piece in _chessPieces)
        {
            piece.SetUserIsOwner(isOwner);
        }
    }

    private void ResetChess()
    {
        ResetPiecesPositionsServerRpc();

        SetWhiteCountServerRpc(0);
        SetBlackCountServerRpc(0);
    }

    private void PieceIsEaten(Transform position, ChessTeam color)
    {
        if (!IsOwner) return;

        if (color == ChessTeam.White)
        {
            if (WhiteEatenCount.Value >= _whiteEatenPlaces.Length) return;
            position.position = _whiteEatenPlaces[WhiteEatenCount.Value].position;
            position.localScale = Vector3.one * 0.3f;
            SetWhiteCountServerRpc(WhiteEatenCount.Value + 1);
        }
        else
        {
            if (BlackEatenCount.Value >= _blackEatenPlaces.Length) return;
            position.position = _blackEatenPlaces[BlackEatenCount.Value].position;
            position.localScale = Vector3.one * 0.3f;
            SetBlackCountServerRpc(BlackEatenCount.Value + 1);
        }
    }

    /// <summary>
    /// parenting as in the source prefab
    /// </summary>
    private void ParentAsInSourcePrefab()
    {
        foreach (var piece in _chessPieces)
        {
            piece.Model.Mesh.transform.SetParent(piece.Model.Parent.transform);
            piece.Model.Mesh.transform.localPosition = piece.Model.LocalPosition;
            piece.Model.Mesh.transform.localRotation = piece.Model.LocalRotation;
        }
    }

    /// <summary>
    /// board is set parent for pieces to avoid incorrect movement for other clients
    /// </summary>
    private void ParentToBoardObject()
    {
        foreach (var piece in _chessPieces)
        {
            piece.Model.Mesh.transform
                .SetParent(transform); 
        }
    }
}
