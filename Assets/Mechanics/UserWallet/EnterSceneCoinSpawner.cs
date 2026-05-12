using Assets.Scripts.API;
using Assets.Scripts.Client;
using Cysharp.Threading.Tasks;
using System;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Zenject;

namespace Assets.Mechanics.UserWallet
{
    public class EnterSceneCoinSpawner : MonoBehaviour
    {
        [SerializeField] private EnterSceneCoin _coinPrefab;
        [SerializeField] private GameObject _info;
        [SerializeField] private LoginPanel _loginPanel;

        //private const string UserStatus = "IsFullFledgedUser"; //the same in ClientIdView.cs & TenHoursNotificationSpawner.cs & CoinInfrastructure.cs

        private EnterSceneCoin _coin;
        private IAccountsService _accountsService;
        private IClientData _clientData;
        private IApiService _apiService;


        [Inject]
        public void Construct(IAccountsService accountsService, IClientData clientData, IApiService apiService)
        {
            _accountsService = accountsService;
            _clientData = clientData;
            _apiService = apiService;
        }

        private void Start()
        {
            _loginPanel.UserLogInSuccess += Refresh;
            _loginPanel.UserLogOut += RemoveAll;

            ShowCoinWithCheck().Forget();
            ShowInfoWithCheck().Forget();
        }

        private void Refresh(string name)
        {
            ShowCoinWithCheck().Forget();
            ShowInfoWithCheck().Forget();
        }

        private void RemoveAll()
        {
            if (_coin != null) Destroy(_coin.gameObject);
            _info.SetActive(false);
        }

        private async UniTask ShowCoinWithCheck()
        {
            if (_coin != null) Destroy(_coin.gameObject);

            DateTimeOffset bonusInitialDate = await _accountsService.GetBonusInitialDate();

            await _apiService.RefreshUserInfo();

            Debug.LogWarning($"bonusInitialDate{bonusInitialDate} >_clientData.CreatedOn {_clientData.CreatedOn}");
            
            if (bonusInitialDate > _clientData.CreatedOn) return;

            bool hasGivenCoin = await _accountsService.GetHasThisUserInitialBonus();
            if (!hasGivenCoin) PutCoinOnScene(); 
        }

        private async UniTask ShowInfoWithCheck()
        {
            bool isFullFledged = await _accountsService.GetIsThisUserFullFledged();

            _info.SetActive(!isFullFledged);
            PlayerPrefs.SetInt(DataExtensions.UserFullStatus + _clientData.UserId, isFullFledged ? 1 : 0);
        }

        private void PutCoinOnScene()
        {
            _coin = Instantiate(_coinPrefab, transform);
            _coin.OnRecieveCoin += RecieveCoinPrefab;
        }

        private void RecieveCoinPrefab()
        {
            if (_coin != null)
            {
                _accountsService.SetThisUserHasInitialBonus().Forget();

                _coin.OnRecieveCoin -= RecieveCoinPrefab;
                Destroy(_coin.gameObject);
            }
        }

        private void OnDestroy()
        {
            _loginPanel.UserLogInSuccess -= Refresh;
            _loginPanel.UserLogOut -= RemoveAll;
        }
    }
}
