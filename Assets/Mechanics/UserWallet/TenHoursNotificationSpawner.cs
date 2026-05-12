using System.Collections;
using Assets.Localization;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace Assets.Mechanics.UserWallet
{
    public class TenHoursNotificationSpawner : MonoBehaviour
    {
        [SerializeField] private ModalWindow _window;
        [SerializeField] private LocalizationVariant _notification;

        //private const string UserStatus = "IsFullFledgedUser"; //the same in ClientIdView.cs & EnterSceneCoinSpawner.cs & CoinInfrastructure.cs

        private IAccountsService _accountsService;
        private IClientData _clientData;
        private Coroutine _checkUserStatus;

        [Inject]
        public void Construct(IAccountsService accountsService, IClientData clientData)
        {
            _accountsService = accountsService;
            _clientData = clientData;
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey(DataExtensions.UserFullStatus + _clientData.UserId))
            {
                var isFullFledged = PlayerPrefs.GetInt(DataExtensions.UserFullStatus + _clientData.UserId) == 1; //1 means is full fledged

                if (isFullFledged) return;
            }

            _checkUserStatus = StartCoroutine(CheckUserStatus());

            GetComponent<LookAtPlayer>().SetPlayerFollow(true);
        }

        private IEnumerator CheckUserStatus()
        {
            while (true)
            {
                CheckUserStatusBackend().Forget();

                yield return new WaitForSeconds(30);
            }
        }

        private async UniTask CheckUserStatusBackend()
        {
            bool isFullFledged = await _accountsService.GetIsThisUserFullFledged();

            if (isFullFledged)
            {
                PlayerPrefs.SetInt(DataExtensions.UserFullStatus + _clientData.UserId, 1);

                ShowModalWindow();

                if (_checkUserStatus != null)
                {
                    StopCoroutine(_checkUserStatus);
                }
            }
        }

        private async void ShowModalWindow()
        {
            await _window.Show($"{_notification.Localize()}", "Ok");
        }

        private void OnDestroy()
        {
            if ( _checkUserStatus != null )
            {
                StopCoroutine(_checkUserStatus);
                _checkUserStatus = null;
            }
        }
    }
}
