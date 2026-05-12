using System;
using System.Collections;
using Assets.Localization;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Environmental;
using Assets.Scripts.Player;
using Assets.Scripts.Shared;
using Infrastructure.Factory;
using Mechanics.Inventory;
using Mechanics.Transactions;
using Mechanics.UserWallet.Model;
using TMPro;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.UserWallet
{
    public class CoinInfrastructure : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private TWRCoinView _twrCoinView;
        [SerializeField] private ViewPanel _radialChangeValueCanvas;
        [SerializeField] private NetworkTriggerObserver _networkTriggerObserver;
        [SerializeField] private InventoryObjectGrabbing _inventoryObjectGrabbing;
        [SerializeField] private GameObject _uiParent;
        [SerializeField] private GameObject _errorCanvas;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private Collider _coinCollider;
        [SerializeField] private Rigidbody[] _rigidBodies;
        [SerializeField] private LocalizationVariant _errorMessage;
        [SerializeField] private LocalizationVariant _defaultErrorMessage;
        [SerializeField] private LocalizationVariant _cantGiveCoinBecauseOfYou;
        [SerializeField] private LocalizationVariant _cantGiveCoinBecauseOfOtherUser;

        #endregion

        //private const string UserStatus = "IsFullFledgedUser"; //the same in TenHoursNotificationSpawner.cs & EnterSceneCoinSpawner.cs & ClienIdView.cs
        private IClientData _clientData;
        private bool _isFullFledgedUser;

        public TransactionInfrastructure TransactionInfrastructure
        {
            get => _transactionInfrastructure;
            set
            {
                _transactionInfrastructure = value;
                _transactionInfrastructure.TransactionInitiatorEnd += OnTransactionEnd;
            }
        }

        public NetworkFactory NetworkFactory
        {
            get => _networkFactory;
            set => _networkFactory = value;
        }

        private TransactionInfrastructure _transactionInfrastructure;

        private NetworkFactory _networkFactory;

        private bool _startTransaction = false;
        private bool _denyTransaction = false;
        private bool _startRemoving = false;

        private Coroutine _hideErrorMessageAfterTimerRoutine;

        public override void OnNetworkSpawn()
        {
            if (OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                _inventoryObjectGrabbing.enabled = false;
                _uiParent.SetActive(false);
                _coinCollider.enabled = false;

                SetKinematic(true);
                XRGrabInteractable grabIneractable = GetComponentInChildren<XRGrabInteractable>();
                if (grabIneractable != null)
                {
                    Destroy(grabIneractable);
                }
            }
            else
            {
                _radialChangeValueCanvas.ShowPanel();
                _networkTriggerObserver.RemoteClientEnterTrigger += OnRemoteClientEnterTrigger;
            }

            if (IsOwner)
            {
                CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
                if (commonDIInstaller != null)
                {
                    _clientData = commonDIInstaller.Resolve<IClientData>();
                }

                if (PlayerPrefs.HasKey(DataExtensions.UserFullStatus + _clientData.UserId))
                {
                    _isFullFledgedUser =
                        PlayerPrefs.GetInt(DataExtensions.UserFullStatus + _clientData.UserId) == 1; //1 means is full fledged
                }
                else
                {
                    _isFullFledgedUser = false;
                }
            }
        }

        private void OnDisable()
        {
            _networkTriggerObserver.RemoteClientEnterTrigger -= OnRemoteClientEnterTrigger;

            if (_transactionInfrastructure == null) return;

            _transactionInfrastructure.TransactionInitiatorEnd -= OnTransactionEnd;
        }

        public int GetWalletValue()
        {
            return (int) _transactionInfrastructure.CurrentUserWallet.sum;
        }

        public bool IsGrabbing()
        {
            return _inventoryObjectGrabbing.IsGrabbing;
        }

        public bool IsFirstContactExist()
        {
            return _inventoryObjectGrabbing.FirstContact;
        }

        public void TrySetCoinValue(int coinValue, string itemName = "", bool showErrorMessage = true)
        {
            if (coinValue > GetWalletValue())
            {
                if (showErrorMessage)
                {
                    string message = _errorMessage.Localize().
                        Replace("{1}", (coinValue - (int)GetWalletValue()).ToString()).
                        Replace("{2}", itemName);

                    HideErrorMessageAfterTimer(message);
                }
            }
            else
            {
                SetCoinValue(coinValue, true);
            }
        }

        public void SetCoinValueToReceive(int coinValue)
        {
            SetCoinValue(coinValue, false);
        }

        private void SetCoinValue(int coinValue, bool checkWallet)
        {
            ChangeManagementCapabilities(false);
            _twrCoinView.SetCoinValue(coinValue, checkWallet);
        }


        public void SetManualMode()
        {
            ChangeManagementCapabilities(true);

            _twrCoinView.DisableRemoteControl();
        }

        public void DeleteCoin()
        {
            _startRemoving = true;
            _networkFactory.DespawnCoin();
        }

        public bool IsRemoving()
        {
            return _startRemoving;
        }

        public void StartGrabbing(GrabbingHand grabbingHand)
        {
            //_inventoryObjectGrabbing.StartGrabbing(grabbingHand);
        }

        private void ChangeManagementCapabilities(bool isVisible)
        {
            _denyTransaction = !isVisible;
            if (isVisible)
            {
                _radialChangeValueCanvas.ShowPanel();
            }
            else
            {
                _radialChangeValueCanvas.HidePanel();
            }
        }

        private void OnTransactionEnd(bool success)
        {
            if (success)
            {
                DeleteCoin();
            }
            else
            {
                _startTransaction = false;
                HideErrorMessageAfterTimer();
            }
        }

        private void OnRemoteClientEnterTrigger(ulong clientID, bool otherUserIsFullFledged)
        {
            if (!otherUserIsFullFledged) //prevent giving coin to other non full fledged user
            {
                HideErrorMessageAfterTimer(_cantGiveCoinBecauseOfOtherUser.Localize());
                return;
            }

            if (!_isFullFledgedUser)
            {
                HideErrorMessageAfterTimer(_cantGiveCoinBecauseOfYou.Localize());
                return;
            }

            if (IsFirstContactExist() && !IsGrabbing() && !_startTransaction && !_denyTransaction)
            {
                _startTransaction = true;
                _transactionInfrastructure.RequestPermissionTransactionWith(clientID, _twrCoinView.CoinValue);

                SetKinematic(true);
            }
        }

        private void HideErrorMessageAfterTimer(string message = "")
        {
            if (_hideErrorMessageAfterTimerRoutine != null)
            {
                StopCoroutine(_hideErrorMessageAfterTimerRoutine);
            }
            _hideErrorMessageAfterTimerRoutine = StartCoroutine(HideErrorMessageAfterTimerRoutine(message));
        }

        private IEnumerator HideErrorMessageAfterTimerRoutine(string message)
        {
            _errorCanvas.SetActive(true);

            if (String.IsNullOrEmpty(message)) _errorText.text = _defaultErrorMessage.Localize();
            else _errorText.text = message;

            yield return new WaitForSeconds(3f);

            _errorCanvas.SetActive(false);

            SetKinematic(false);

            _hideErrorMessageAfterTimerRoutine = null;
        }

        private void SetKinematic(bool isKinematic)
        {
            foreach (Rigidbody rb in _rigidBodies)
            {
                rb.isKinematic = isKinematic;
            }
        }
    }
}