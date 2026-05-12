using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.TowerObjects;
using Assets.Scripts.Trading.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Mechanics.UserWallet.Service;
using UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Trading
{
    public class TradeService : ITradeService
    {
        private IClientData _clientData;
        private IWalletService _walletService;
        private ITowerObjectsService _towerObjectsService;

        private bool _isTransactionInProgress;
        private IServerApiData _serverApiData;

        public event Action TransactionCompleted;

        [Inject]
        public void Construct(IClientData clientData, IServerApiData serverApiData, ITowerObjectsService towerObjectsService,
            IWalletService walletService)
        {
            _serverApiData = serverApiData;
            _clientData = clientData;
            _walletService = walletService;
            _towerObjectsService = towerObjectsService;
        }

        public async UniTask<ShopDto[]> GetShops()
        {
            var utcs = new UniTaskCompletionSource<ShopDto[]>();

            string url = string.Empty;
            HttpResponse<RequestDto<ShopDto[]>> httpResponse = null;
#if !UNITY_SERVER
            await UniTask.WaitUntil(() => APIService.GetShopsUrl != string.Empty);

            url = APIService.GetShopsUrl;
            url = APIService.AddLanguageParameter(url);
            httpResponse = await WebRequestFunctions.GetWithDeserialization<RequestDto<ShopDto[]>>
                    (url, _clientData.AccessToken);
#endif
#if UNITY_SERVER
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetShopsUrl));
            url = ServerApiService.GetShopsUrl;
            httpResponse = await WebRequestFunctions.GetWithDeserialization<RequestDto<ShopDto[]>>
                (url, _serverApiData.AccessToken);  
#endif

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(Array.Empty<ShopDto>());
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetShops)}. Cannot get Shops. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(Array.Empty<ShopDto>());
            }

            return await utcs.Task;
        }

        public async UniTask<bool> BuySpaceAccess(Guid spaceId)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            string url = APIService.SpaceAccessPaymentsUrl;
            
            SpaceAccessPaymentsDTO spaceAccessPayments = new();
            spaceAccessPayments.SpaceId = spaceId.ToString();
            
            HttpResponse<string> httpResponse =
                await WebRequestFunctions.Post(url, spaceAccessPayments, _clientData.AccessToken);
            
            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> BuyItem(ShopItemDto shopItemDto)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            if (shopItemDto.Item == null)
            {
                Debug.LogError("shopItemDto.Item == null");
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            ContractDto contract = new()
            {
                ObjectId = shopItemDto.Item.Id,
                DoesUserBuysAnItem = true,
            };

            if (_isTransactionInProgress)
            {
                await UniTask.WaitWhile(() => _isTransactionInProgress);
            }

            _isTransactionInProgress = true;

            TowerObjectDto[] userItems = await _towerObjectsService.GetUserObjects();

            if (shopItemDto.Item == null)
            {
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            if (userItems.FirstOrDefault((item) => item.Id == shopItemDto.Item.Id)
                != null)
            {
                Debug.LogError("User has the same item");
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            var userWallet = await _walletService.GetMyWalletAsync();
            if (userWallet == null)
            {
                Debug.LogError("Can't get user wallet");
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            //if (userWallet.sum < shopItemDto.Price)
            //{
            //    Debug.LogError("Not enough money");
            //    _isTransactionInProgress = false;
            //    utcs.TrySetResult(false);
            //    return await utcs.Task;
            //}

            if (shopItemDto.Item == null)
            {
                Debug.LogError("shopItemDto.Item == null");
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            if (!await RequestBuySellItem(contract))
            {
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }
            
            _isTransactionInProgress = false;
            Debug.Log("Item bought!");
            utcs.TrySetResult(true);

            return await utcs.Task;
        }

        public async UniTask<bool> SellItem(Guid itemId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            if (_isTransactionInProgress)
            {
                await UniTask.WaitWhile(() => _isTransactionInProgress);
            }

            _isTransactionInProgress = true;

            ContractDto contract = new()
            {
                ObjectId = itemId,
                DoesUserBuysAnItem = false,
            };

            if (!await RequestBuySellItem(contract))
            {
                _isTransactionInProgress = false;
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            _isTransactionInProgress = false;
            Debug.Log("Item sold!");
            utcs.TrySetResult(true);

            return await utcs.Task;
        }

        private async UniTask<bool> RequestBuySellItem(ContractDto contract)
        {            
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.GetBuySellContractsUrl != null);

            string url = APIService.GetBuySellContractsUrl;

            HttpResponse<string> httpResponse =
                await WebRequestFunctions.Post(url, contract, _clientData.AccessToken);
            
            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                utcs.TrySetResult(true);
                TransactionCompleted?.Invoke();
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestBuySellItem)}. Cannot buy/sell item. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

    }
}