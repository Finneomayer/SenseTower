using System;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.Doors
{
    public class DoorPaidController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private ModalWindow _modalWindow;

        #endregion
        
        private string _paidText =
            "Вход в помещение платный. Согласны ли Вы потратить {0} TWR, чтобы получить доступ в это помещение на 24 часа?";

        private IClientData _clientData;
        private ITradeService _tradeService;

        private bool _resultAccess;

        [Inject]
        private void Construct(IClientData clientData, ITradeService tradeService)
        {
            _clientData = clientData;
            _tradeService = tradeService;
        }

        public bool AllowAccess()
        {
            return _resultAccess;
        }

        public async UniTask<bool> CheckDoorAccess(LocalSpace _myPlace)
        {
            var untc = new UniTaskCompletionSource<bool>();
            Debug.LogWarning($"Door click {transform.name}");
            bool modalWindowResult = false;
            bool spaceIsPaid = _myPlace != null && _myPlace.PublicAccessType == SpaceAccessType.Paid;

            if (_myPlace != null)
            {
                if (_clientData.UserId == _myPlace.SpaceOwner.UserId || _myPlace.AdminIds.Contains(_clientData.UserId.ToString()) )
                {
                    untc.TrySetResult(true);
                    return await untc.Task;
                }
            }
            
            if (spaceIsPaid)
            {
                if (_myPlace.PublicAccessModeSettings.PaymentAccessModeSettings.IsPaid)
                {
                    untc.TrySetResult(true);
                }
                else
                {
                    var tax = _myPlace.PublicAccessModeSettings.PaymentAccessModeSettings.DailyTax;
                    modalWindowResult = await _modalWindow.Show(string.Format(_paidText, tax), "Оплатить", "Отмена");

                    if (modalWindowResult)
                    {
                        _modalWindow.gameObject.SetActive(false);
                        bool buyResult = await _tradeService.BuySpaceAccess(_myPlace.Id);

                        if (buyResult)
                        {
                            untc.TrySetResult(true);
                        }
                        else
                        {
                            _modalWindow.gameObject.SetActive(true);
                            untc.TrySetResult(false);
                            
                            await _modalWindow.Show("Доступ в помещение купить не удалось","Закрыть");
                        }
                    }
                }
            }
            else
            {
                untc.TrySetResult(true);

            }

            return await untc.Task;
        }
    }
}