using Assets.Scripts.Player;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Mechanics.FriendsList
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class AddingFriendCollider : MonoBehaviour
    {
        public PlayerController HandController { get; private set; }
        public string CurrentFriendingUsername => _currentFriendingClientIdView != null
            ? _currentFriendingClientIdView.PlayerLogin : string.Empty;
        //private ulong _currentFriendingClientId;
        private AddingFriendRemoteCollider _currentFriendingCollider;

        private ClientIdView _currentFriendingClientIdView;

        public Action<AddingFriendCollider, ulong> FriendingStarted;
        public Action<AddingFriendCollider> FriendingStopped;

        //private void OnEnable()
        //{
        //    RegisterListeners();
        //}

        //private void OnDisable()
        //{
        //    UnregisterListeners();
        //}

        private void Awake()
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_currentFriendingCollider != null)
            {
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                return;
            }

            if (!other.gameObject.TryGetComponent(out AddingFriendRemoteCollider otherFriendCollider))
            {
                return;
            }

            NetworkObject otherPlayer = otherFriendCollider.GetComponentInParent<NetworkObject>();

            if (otherPlayer == null || otherPlayer.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.LogWarning("You are friending with youself");
                return;
            }
            // TODO: Temporary. To check, if user is in friend list
            _currentFriendingClientIdView = otherPlayer.GetComponent<ClientIdView>();

            _currentFriendingCollider = otherFriendCollider;
            //_currentFriendingClientId = otherPlayer.OwnerClientId;
            FriendingStarted?.Invoke(this, otherPlayer.OwnerClientId);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_currentFriendingCollider == null || other.gameObject != _currentFriendingCollider.gameObject)
            {
                return;
            }

            StopFriending();
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (_currentFriendingCollider == null || other.gameObject != _currentFriendingCollider.gameObject)
        //    {
        //        return;
        //    }
        //}

        public void Init(PlayerController handController)
        {
            HandController = handController;
            //UnregisterListeners();
            StopFriending();
            //_grabbingHand = grabbingHand;
            //RegisterListeners();
        }

        //private void RegisterListeners()
        //{
        //    UnregisterListeners();
        //    if (_grabbingHand != null)
        //    {
        //        _grabbingHand.GrabbingStarted += _grabbingHand_GrabbingStarted;
        //        _grabbingHand.GrabbingStopped += _grabbingHand_GrabbingStopped;
        //    }
        //}

        //private void UnregisterListeners()
        //{
        //    if (_grabbingHand != null)
        //    {
        //        _grabbingHand.GrabbingStarted -= _grabbingHand_GrabbingStarted;
        //    }
        //}

        //private void _grabbingHand_GrabbingStarted(GrabbingHand obj)
        //{
        //    if (_currentFriendingCollider == null)
        //    {
        //        return;
        //    }
        //    FriendingStarted?.Invoke(this, _currentFriendingClientId);
        //}

        //private void _grabbingHand_GrabbingStopped(GrabbingHand obj)
        //{
        //    if (_currentFriendingCollider == null)
        //    {
        //        return;
        //    }
        //    FriendingStopped?.Invoke(this);
        //}

        private void StopFriending()
        {
            FriendingStopped?.Invoke(this);
            _currentFriendingCollider = null;
        }
    }
}