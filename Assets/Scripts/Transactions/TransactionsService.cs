using System;
using System.IO;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        private IClientData _clientData;

        [Inject]
        public void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<TransactionDto[]> GetTransactions(int? limit = null, int? offset = null)
        {
            if (limit.HasValue && limit.Value <= 0)
            {
                var utcs = new UniTaskCompletionSource<TransactionDto[]>();
                utcs.TrySetResult(new TransactionDto[0]);
                return await utcs.Task;
            }

            return await RequestTransactions(limit, offset);
        }

        public async UniTask<TransactionDto[]> GetTransactionsByType(Enumenators.TransactionPurposeTypeDto[] twrOperationTypes,
            int? limit = null, int? offset = null)
        {
            if (limit.HasValue && limit.Value <= 0)
            {
                var utcs = new UniTaskCompletionSource<TransactionDto[]>();
                utcs.TrySetResult(new TransactionDto[0]);
                return await utcs.Task;
            }

            return await RequestTransactions(limit, offset, twrOperationTypes);
        }

        private async UniTask<TransactionDto[]> RequestTransactions(int? limit = null, int? offset = null,
            Enumenators.TransactionPurposeTypeDto[] twrOperationTypes = null)
        {
            await UniTask.WaitUntil(() => APIService.GetTransactionsUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<TransactionDto[]>();

            string url = APIService.GetTransactionsUrl;

            if (twrOperationTypes != null)
            {
                foreach (var operationType in twrOperationTypes)
                {
                    url = APIService.AddParameter(url, $"TransactionTypes={(int)operationType}");
                }
            }

            if (offset.HasValue)
            {
                url = APIService.AddParameter(url, $"Offset={offset.Value}");
            }
            if (limit.HasValue)
            {
                url = APIService.AddParameter(url, $"Limit={limit.Value}");
            }

            url = APIService.AddLanguageParameter(url);

            HttpResponse<TransactionResponse> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<TransactionResponse>(url,
                    _clientData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(new TransactionDto[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestTransactions)}. Cannot get Transactions. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new TransactionDto[0]);
            }

            return await utcs.Task;
        }

        private class TransactionResponse
        {
            public TransactionDto[] Result;
            public string Error;
        }
    }
}