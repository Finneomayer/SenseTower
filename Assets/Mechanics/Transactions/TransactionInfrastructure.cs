using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Mechanics.UserWallet.Model;
using Mechanics.UserWallet.Service;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Mechanics.Transactions
{
    public class TransactionInfrastructure : NetworkBehaviour
    {
        public event Action<bool> TransactionRecipientEnd;
        public event Action<bool> TransactionInitiatorEnd;
        [field: SerializeField] public TwrWallet CurrentUserWallet { get; private set; }
        private IClientData _clientData;
        private IWalletService _walletService;
        private List<TransactionData> _transactionList = new();

        [Inject]
        private void Construct(IClientData clientData, IWalletService walletService)
        {
            _walletService = walletService;
            _clientData = clientData;
        }

        private async void Awake()
        {
#if !UNITY_SERVER
            CurrentUserWallet = await GetWallet();
            SetCurrentUserWalletValue();
#endif
        }

        public void CurrentUserSuccesOperationActionInvoke()
        {
            TransactionInitiatorEnd?.Invoke(true);
        }

        public void RequestPermissionTransactionWith(ulong serverId, decimal amount)
        {
            if (CurrentUserWallet.sum < amount || CurrentUserWallet.sum == 0)
            {
                TransactionInitiatorEnd?.Invoke(false);
                return;
            }

            TransactionData tempTransaction = new TransactionData();
            tempTransaction.TransactionID = String.Empty;
            tempTransaction.Recipient = String.Empty;
            tempTransaction.Initiator = _clientData.UserId.ToString();
            tempTransaction.TransactionType = WalletOwnerType.User;
            tempTransaction.Amount = amount;

            RequestPermissionTransactionServerRPC(remoteUserId: serverId, tempTransaction);
        }

        private async void SetCurrentUserWalletValue()
        {
            CurrentUserWallet = await GetWallet();
        }

        private async UniTask<TwrWallet> GetWallet()
        {
            var untc = new UniTaskCompletionSource<TwrWallet>();
            CurrentUserWallet = await _walletService.GetMyWalletAsync();

            if (CurrentUserWallet == null)
                untc.TrySetResult(null);
            else
                untc.TrySetResult(CurrentUserWallet);

            return await untc.Task;
        }

        #region Client

        [ClientRpc]
        private void GetRecipientTransactionClientRpc(ulong userId, TransactionData transaction)
        {
            if (userId != NetworkManager.Singleton.LocalClientId) return;

            transaction.Recipient = _clientData.UserId.ToString();

            StartTransactionServerRpc(transaction);
        }

        [ClientRpc]
        private void StartTransactionClientRPC(TransactionData transaction)
        {
            string userId = _clientData.UserId.ToString();
            if (userId.Equals(transaction.Initiator))
            {
                SendTransactionCommand(transaction);
            }
        }

        [ClientRpc]
        private void OnEndTransactionClientRPC(TransactionData transaction)
        {
            string userId = _clientData.UserId.ToString();
            SetCurrentUserWalletValue();
            if (userId.Equals(transaction.Recipient))
            {
                TransactionRecipientEnd?.Invoke(transaction.Success);
            }
            else if (userId.Equals(transaction.Initiator))
            {
                TransactionInitiatorEnd?.Invoke(transaction.Success);
            }
        }

        private async void SendTransactionCommand(TransactionData transaction)
        {
            TransferTwrsCommand tempTransaction = new();
            tempTransaction.Amount = transaction.Amount;
            tempTransaction.ReceiverId = transaction.Recipient;
            tempTransaction.ReceiverType = transaction.TransactionType;
            tempTransaction.SenderType = WalletOwnerType.User;
            tempTransaction.HoldTransfer = false;
            
            RequestHelper options = new RequestHelper();
            options.Uri = APIService.GetWalletUrl + "/transfer";
            options.BodyString = JsonConvert.SerializeObject(tempTransaction);
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            var result = await WebRequestFunctions.PostWithDeserialization<ScResult<TwrsTransferTransaction>>(options);

            transaction.Success = result.ResponseCode == HttpResponse<ScResult<TwrsTransferTransaction>>.SuccessCode;

            OnEndTransactionServerRpc(transaction);
        }

        #endregion

        #region Server

        [ServerRpc(RequireOwnership = false)]
        private void RequestPermissionTransactionServerRPC(ulong remoteUserId, TransactionData transaction)
        {
            Guid transactionId = Guid.NewGuid();

            transaction.TransactionID = transactionId.ToString();
            transaction.ReceiptAllowed = true;

            _transactionList.Add(transaction);

            GetRecipientTransactionClientRpc(remoteUserId, transaction);
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartTransactionServerRpc(TransactionData transaction)
        {
            StartTransactionClientRPC(transaction);
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnEndTransactionServerRpc(TransactionData transaction)
        {
            var tempElement =
                _transactionList.FirstOrDefault(element => element.TransactionID == transaction.TransactionID);
            if (string.IsNullOrEmpty(tempElement.TransactionID))
                tempElement = transaction;

            OnEndTransactionClientRPC(transaction);
        }

        #endregion

        #region InnerClass

        public struct TransactionData : INetworkSerializable
        {
            public string TransactionID;
            public string Initiator;
            public WalletOwnerType TransactionType;
            public string Recipient;
            public decimal Amount;
            public bool Success;
            public bool ReceiptAllowed;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref TransactionID);
                serializer.SerializeValue(ref Initiator);
                serializer.SerializeValue(ref TransactionType);
                serializer.SerializeValue(ref Recipient);
                serializer.SerializeValue(ref Amount);
                serializer.SerializeValue(ref Success);
                serializer.SerializeValue(ref ReceiptAllowed);
            }
        }

        #endregion
    }
}