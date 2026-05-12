using Assets.Mechanics.FriendsList;
using Assets.Mechanics.FriendsList.Models;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Hall;
using Assets.Scripts.Shared;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Mechanics.SignalBusModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Mechanics.FriendsList
{
    public class AddingFriendHandler : MonoBehaviour
    {
        [Serializable]
        private class AddingFriendEffect
        {
            public ParticleSystem ParticleSystem;
            public AudioSource AudioSource;
        }

        [SerializeField]
        private ulong FriendingInEditorClientId;
        [SerializeField]
        private string FriendingInEditorClientName;
        [SerializeField]
        private float FriendingDelay = 1;
        [SerializeField]
        private float FriendingTime = 3;
        [SerializeField]
        private float HapticPeriod = 0.5f;
        [SerializeField]
        private float HapticDuration = 0.3f;
        [SerializeField]
        private float HapticAmplitude = 1;

        [SerializeField]
        private AddingFriendEffect AddingFriendCompleteEffect;

        private AddingFriendCollider[] _addingFriendColliders = new AddingFriendCollider[0];

        private AddingFriendCollider _currentAddingFriendCollider;
        private Coroutine _friendingProgressRoutine;
        private PlayerEmoji _playerEmoji;
        private NetworkFriendListService _networkFriendListService;
        private List<Guid> _sendedRequestsUserIds = new();

        private IFriendsService _friendsService;
        private IClientData _clientData;
        private IAccountsService _accountsService;
        private SignalBus _signalBus;

        private void Awake()
        {
            SceneInstaller sceneInstaller = FindObjectOfType<SceneInstaller>();
            if (sceneInstaller != null)
            {
                _networkFriendListService = sceneInstaller.Resolve<NetworkFriendListService>();
                _signalBus = sceneInstaller.Resolve<SignalBus>();
            }

            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _friendsService = commonDIInstaller.Resolve<IFriendsService>();
                _clientData = commonDIInstaller.Resolve<IClientData>();
                _accountsService = commonDIInstaller.Resolve<IAccountsService>();
            }
        }

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
        }

        public void Init(IEnumerable<AddingFriendCollider> addingFriendColliders, PlayerEmoji playerEmoji)
        {
            StopFriending();
            UnregisterListeners();

            _addingFriendColliders = addingFriendColliders.ToArray();
            _playerEmoji = playerEmoji;
            if (gameObject.activeInHierarchy)
            {
                RegisterListeners();
            }
        }

        private void RegisterListeners()
        {
            UnregisterListeners();
            foreach (var item in _addingFriendColliders)
            {
                item.FriendingStarted += OnFriendingStarted;
                item.FriendingStopped += OnFriendingStopped;
            }

            _signalBus?.Subscribe<RemoveFromFriendListRequestSignal>(OnRemoveFromFriendListSignalReceived);
        }

        private void UnregisterListeners()
        {
            foreach (var item in _addingFriendColliders)
            {
                item.FriendingStarted -= OnFriendingStarted;
                item.FriendingStopped -= OnFriendingStopped;
            }

            _signalBus?.TryUnsubscribe<RemoveFromFriendListRequestSignal>(OnRemoveFromFriendListSignalReceived);
        }

        [ContextMenu("Start Friending In Editor")]
        private void StartFriendingInEditor()
        {
            _currentAddingFriendCollider = _addingFriendColliders[0];
            _friendingProgressRoutine = StartCoroutine(
                FriendingProgressRoutine(FriendingInEditorClientId, FriendingInEditorClientName));
        }

        private void StopFriending()
        {
            if (_friendingProgressRoutine != null)
            {
                _playerEmoji.StopEmojiEffect(EmojiType.Friending);
                StopCoroutine(_friendingProgressRoutine);
                _friendingProgressRoutine = null;
                _currentAddingFriendCollider = null;
            }
        }

        private void OnFriendingStarted(AddingFriendCollider addingFriendCollider, ulong friendingClientId)
        {
            if (_currentAddingFriendCollider != null)
            {
                return;
            }

            if (addingFriendCollider == null || addingFriendCollider.CurrentFriendingUsername == null)
            {
                return;
            }

            _currentAddingFriendCollider = addingFriendCollider;
            _friendingProgressRoutine = StartCoroutine(
                FriendingProgressRoutine(friendingClientId, addingFriendCollider.CurrentFriendingUsername));
        }

        private void OnFriendingStopped(AddingFriendCollider addingFriendCollider)
        {
            if (addingFriendCollider == null)
            {
                return;
            }

            if (_currentAddingFriendCollider != addingFriendCollider)
            {
                return;
            }

            StopFriending();
        }

        private void OnRemoveFromFriendListSignalReceived(RemoveFromFriendListRequestSignal removeFromFriendListRequestSignal)
        {
            if (removeFromFriendListRequestSignal == null)
            {
                return;
            }
            _sendedRequestsUserIds.Remove(removeFromFriendListRequestSignal.UserId);
            Debug.Log($"Removed from friends {removeFromFriendListRequestSignal.UserId}");
        }

        public IEnumerator FriendingProgressRoutine(ulong friendingClientId, string friendingUserName)
        {
            yield return new WaitForSeconds(FriendingDelay);

            if (_friendsService == null || _clientData == null || _accountsService == null)
            {
                StopFriending();
                yield break;
            }

            Task<UserLookupInfo[]> getUsersLookupInfoTask = _accountsService.GetUsersLookupInfo().AsTask();
            yield return new WaitUntil(() => getUsersLookupInfoTask.IsCompleted);

            if (getUsersLookupInfoTask.Result == null)
            {
                StopFriending();
                yield break;
            }

            UserLookupInfo friendingUserInfo = getUsersLookupInfoTask.Result.FirstOrDefault(
                u => u.Name.Equals(friendingUserName, StringComparison.OrdinalIgnoreCase));

            if (friendingUserInfo == null)
            {
                StopFriending();
                yield break;
            }

            if (_sendedRequestsUserIds.Contains(friendingUserInfo.Id))
            {
                StopFriending();
                yield break;
            }

            Task<GetFriendDTO[]> getFriendsTask = _friendsService.GetFriendsList(_clientData.UserId.ToString()).AsTask();
            yield return new WaitUntil(() => getFriendsTask.IsCompleted);

            if (getFriendsTask.Result == null)
            {
                StopFriending();
                yield break;
            }

            foreach (var friend in getFriendsTask.Result)
            {
                if (friend.UserId == friendingUserInfo.Id)
                {
                    StopFriending();
                    yield break;
                }
            }

            _playerEmoji.StartEmojiEffect(EmojiType.Friending);

            float friendingCompleteTime = Time.time + FriendingTime;

            while (Time.time < friendingCompleteTime)
            {
                if (_currentAddingFriendCollider != null && _currentAddingFriendCollider.HandController != null
                    && _currentAddingFriendCollider.HandController.gameObject.activeInHierarchy)
                {
                    _currentAddingFriendCollider.HandController.GrabInteractor.SendHapticImpulse(HapticAmplitude, HapticDuration);
                }
                yield return new WaitForSeconds(HapticPeriod);
            }

            if (_networkFriendListService != null)
            {
                _networkFriendListService.TryMakeFriend(friendingClientId);
            }

            _sendedRequestsUserIds.Add(friendingUserInfo.Id);

            _playerEmoji.StopEmojiEffect(EmojiType.Friending);

            AddingFriendCompleteEffect.ParticleSystem.transform.position = _currentAddingFriendCollider.transform.position;
            AddingFriendCompleteEffect.AudioSource.Play();
            AddingFriendCompleteEffect.ParticleSystem.Play();

            _friendingProgressRoutine = null;
            _currentAddingFriendCollider = null;
        }
    }
}