using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mechanics.Network.Scripts;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.Shop.Scripts
{
    public class NetworkShopService : NetworkBehaviour
    {
        #region Inspector

        [SerializeField] private ShopItemsView _shopItemsView;
        [SerializeField] private ShopDto[] _shops;

        #endregion

        private ShopDto _currentShop;
        private ITradeService _tradeService;
        private IServerApiService _serverApiService;
        private Dictionary<ulong, int> _pickedObjects = new();
        private bool _shopRequestInProgress;

        [Inject]
        private void Construct(ITradeService tradeService, IServerApiService serverApiService)
        {
            _serverApiService = serverApiService;
            _tradeService = tradeService;

            _serverApiService.ServerAuth += OnAuthServer;
        }

        private void OnEnable()
        {
#if !UNITY_SERVER
            _shopItemsView.ItemGrab += OnItemStartGrab;
            _shopItemsView.ReturnToShop += OnReturnToShop;
            _shopItemsView.ShopInitialize += OnShopInitialize;
            _tradeService.TransactionCompleted += OnTradeTransactionCompleted;
#endif
        }

        private void OnDisable()
        {
#if UNITY_SERVER
            _serverApiService.ServerAuth -= OnAuthServer;
#endif

#if !UNITY_SERVER
            _shopItemsView.ItemGrab -= OnItemStartGrab;
            _shopItemsView.ReturnToShop -= OnReturnToShop;
            _shopItemsView.ShopInitialize -= OnShopInitialize;

            _tradeService.TransactionCompleted -= OnTradeTransactionCompleted;
#endif
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                GetShopsForCurrentUserServerRpc(NetworkManager.Singleton.LocalClientId);
            }

#if UNITY_SERVER
            RefreshingShopItems().Forget();
            NetworkEventsManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
#endif
        }

        public override void OnNetworkDespawn()
        {
#if UNITY_SERVER
            NetworkEventsManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
#endif
        }

        #region ServerRpc

        [ServerRpc(RequireOwnership = false)]
        public void UpdateShopObjectServerRpc(ulong clientId)
        {
            RemoveUserFromList(clientId);
            UpdateShopsInfo().Forget();
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetPickedObjectsServerRpc()
        {
            UpdatePickedInfo();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPickedObjectsServerRpc(ulong clientId, int itemNumberPlace)
        {
            _pickedObjects[clientId] = itemNumberPlace;
            SetShopObjectDisableClientRpc(itemNumberPlace);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GetShopsForCurrentUserServerRpc(ulong clientId)
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = new[] { clientId };

            SetShopsClientRpc(PackageAllShopsData(), _pickedObjects.Values.ToArray(), clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateShopsInfoServerRpc(ulong clientId)
        {
            RemoveUserFromList(clientId);
            UpdateShopsInfo().Forget();
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        private void SetShopsClientRpc(string data, int[] pickedIds, ClientRpcParams clientRpcParams = default)
        {
            _currentShop = JsonConvert.DeserializeObject<ShopDto>(data);
            _shopItemsView.Init(_currentShop, pickedIds);
        }

        [ClientRpc]
        private void SetShopObjectDisableClientRpc(int placeNumber)
        {
            _shopItemsView.DisablePlaceNumberObject(placeNumber);
        }

        #endregion

        private void OnShopInitialize()
        {
            GetPickedObjectsServerRpc();
        }

        private void OnReturnToShop()
        {
            UpdateShopObjectServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        private async void OnAuthServer()
        {
            _shops = await GetShops();
        }

        private void OnTradeTransactionCompleted()
        {
            UpdateShopsInfoServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        private string PackageAllShopsData()
        {
            ShopDto currentShop = _shops[0];

            string sendData = JsonConvert.SerializeObject(currentShop);
            return sendData;
        }

        private string PackageShopsDataWithoutPicked()
        {
            ShopDto currentShop = _shops[0];
            ShopItemDto[] shopItemDto =
                currentShop.Items.Where(item => _pickedObjects.Any(picked => picked.Value == item.PlaceNumber) == false)
                    .ToArray();

            currentShop.Items = shopItemDto;
            Debug.Log(currentShop.Items.Length);
            string sendData = JsonConvert.SerializeObject(currentShop);
            return sendData;
        }

        /// <summary>
        /// Invoke only on server
        /// </summary>
        private async UniTask UpdateShopsInfo()
        {
            _shops = await GetShops();

            SetShopsForAllUsers();
            UpdatePickedInfo();
        }

        private void UpdatePickedInfo()
        {
            for (int i = _pickedObjects.Count - 1; i >= 0; i--)
            {
                ulong clientId = _pickedObjects.Keys.ElementAt(i);
                if (!NetworkManager.ConnectedClientsIds.Contains(_pickedObjects.Keys.ElementAt(i)))
                {
                    _pickedObjects.Remove(clientId);
                }
            }

            if (_pickedObjects.Count == 0)
            {
                SetShopObjectDisableClientRpc(-1);
            }
            else
            {
                foreach (KeyValuePair<ulong, int> pickedObject in _pickedObjects)
                {
                    SetShopObjectDisableClientRpc(pickedObject.Value);
                }
            }
        }

        /// <summary>
        /// Invoke only on server
        /// </summary>
        private async UniTask<ShopDto[]> GetShops()
        {
            if (_shopRequestInProgress)
            {
                await UniTask.WaitUntil(() => !_shopRequestInProgress);
                return _shops;
            }

            _shopRequestInProgress = true;

            ShopDto[] shops = null;
            shops = await _tradeService.GetShops();
            while (shops == null || shops.Length == 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5));
                shops = await _tradeService.GetShops();
            }

            _shopRequestInProgress = false;

            return shops;
        }

        /// <summary>
        /// Invoke only on server
        /// </summary>
        private void SetShopsForAllUsers()
        {
            ClientRpcParams clientRpcParams = new();
            clientRpcParams.Send.TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds;

            SetShopsClientRpc(PackageAllShopsData(), _pickedObjects.Values.ToArray(), clientRpcParams);
        }

        private void OnItemStartGrab(ShopItemDto shopItem)
        {
            if (shopItem != null)
                SetPickedObjectsServerRpc(NetworkManager.Singleton.LocalClientId, shopItem.PlaceNumber);
        }

        private void OnClientDisconnect(ulong disconnectClientId)
        {
            RemoveUserFromList(disconnectClientId);
            UpdateShopsInfo().Forget();
        }

        private void RemoveUserFromList(ulong clientId)
        {
            if (_pickedObjects.ContainsKey(clientId))
                _pickedObjects.Remove(clientId);
        }

        private async UniTask RefreshingShopItems()
        {
            while (NetworkManager != null)
            {
                var shops = await GetShops();
                if (shops == null || AreShopsEqual(_shops, shops))
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(5));
                    continue;
                }

                _shops = shops;

                SetShopsForAllUsers();
                UpdatePickedInfo();
                await UniTask.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private bool AreShopsEqual(ShopDto[] shops1, ShopDto[] shops2)
        {
            if (shops1 == null && shops2 != null
                || shops1 != null && shops2 == null)
            {
                return false;
            }

            if (shops1 != null)
            {
                if (shops1.Length != shops2.Length)
                {
                    return false;
                }
                for (int i = 0; i < shops1.Length; i++)
                {
                    if (!shops1[i].IsEqual(shops2[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}