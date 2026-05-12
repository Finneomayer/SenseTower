using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Mechanics.UserWallet.Model;
using Models;
using System;
using UnityEngine;
using Zenject;

namespace Mechanics.UserWallet.Service
{
    public class WalletService : IWalletService
    {
        public TwrWallet TwrWallet { get; private set; } = new();
        private IApiService _apiService;
        private IClientData _clientData;
        private bool _getMyWalletRequestingInProgress;

        public event Action WalletChanged;

        [Inject]
        private void Construct(IApiService apiService, IClientData clientData)
        {
            _apiService = apiService;
            _clientData = clientData;

#if !UNITY_SERVER
            RefreshingWalletRoutine().Forget();
#endif
        }

        private async UniTask RefreshingWalletRoutine()
        {
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(_clientData.AccessToken));

            TwrWallet = await GetMyWalletAsync();
            decimal previousSum = TwrWallet.sum;

            while (true)
            {
                await UniTask.Delay(5000);

                TwrWallet = await GetMyWalletAsync();
                if (previousSum != TwrWallet.sum)
                {
                    previousSum = TwrWallet.sum;
                    WalletChanged?.Invoke();
                }
            }           
        }

        public async UniTask<TwrWallet> GetMyWalletAsync()
        {
            if (_getMyWalletRequestingInProgress)
            {
                await UniTask.WaitWhile(() => _getMyWalletRequestingInProgress);
            }
            _getMyWalletRequestingInProgress = true;

            var utcs = new UniTaskCompletionSource<TwrWallet>();

            var result = await _apiService.GetWithAuthToken<ScResult<decimal>>(APIService.GetWalletUrl);

            if (result == null)
            {
                TwrWallet = new TwrWallet();
            }
            else
            {
                TwrWallet = new TwrWallet() { sum = result.Result };
            }
            utcs.TrySetResult(TwrWallet);

            _getMyWalletRequestingInProgress = false;

            return await utcs.Task;
        }
    }
}