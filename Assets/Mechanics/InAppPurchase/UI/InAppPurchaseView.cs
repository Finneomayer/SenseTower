using System;
using System.Collections;
using System.Threading.Tasks;
using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.API;
using Cysharp.Threading.Tasks;
using Mechanics.InAppPurchase.Interfaces;
using UI;
using UnityEngine;
using Zenject;

namespace Mechanics.InAppPurchase.UI
{
    public class InAppPurchaseView : ViewPanel
    {
        #region Inspector

        [SerializeField] private GameObject _contentParent;
        [SerializeField] private AppPurchaseViewElement _appPurchaseViewElementPrefab;
        [SerializeField] private GameObject _textLink;
        [SerializeField] private GameObject _waitingPanel;

        #endregion

        private IAppPurchaseService _appPurchaseService;

        [Inject]
        private void Construct(IAppPurchaseService appPurchaseService)
        {
            _appPurchaseService = appPurchaseService;
            _appPurchaseService.ConstructPurchaseList();
        }

        public override void ShowPanel()
        {
            FillInfoPanel();
            base.ShowPanel();
        }

        private async void FillInfoPanel()
        {
            ClearPreviousValue();
            _waitingPanel.SetActive(true);

            string[] purchases = await _appPurchaseService.GetAvailablePurchase();

            _waitingPanel.SetActive(false);

            if (purchases.Length == 0)
            {
                //var payment = Instantiate(_appPurchaseViewElementPrefab);
                //payment.transform.SetParent(_contentParent.transform);
                //payment.SetInfo("Купить TWR");
                //payment.transform.localPosition = Vector3.zero;
                //payment.transform.localScale = Vector3.one;
                //payment.transform.localRotation = Quaternion.identity;

                //payment.SubscribeToBuyButtonClick((tittle) => { StartLocalPayment(tittle); });

                var link = Instantiate(_textLink);
                link.transform.SetParent(_contentParent.transform);
                link.transform.localPosition = Vector3.zero;
                link.transform.localScale = Vector3.one;
                link.transform.localRotation = Quaternion.identity;
            }
            else
            {
                for (int i = 0; i < purchases.Length; i++)
                {
                    var instance = Instantiate(_appPurchaseViewElementPrefab);
                    instance.transform.SetParent(_contentParent.transform);
                    instance.SetInfo(purchases[i]);

                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localScale = Vector3.one;
                    instance.transform.localRotation = Quaternion.identity;

                    instance.SubscribeToBuyButtonClick((tittle) => { StartPurchase(tittle); });
                }
            }
        }

        private void ClearPreviousValue()
        {
            for (int i = 0; i < _contentParent.transform.childCount; i++)
            {
                Destroy(_contentParent.transform.GetChild(i).gameObject);
            }
        }

        private void StartPurchase(string tittle)
        {
            _appPurchaseService.PurchaseByName(tittle,
                (result, itemTittle) => { OnPurchaseResult(result, itemTittle); });
        }

        private void StartLocalPayment(string tittle)
        {
            Application.OpenURL(APIService.GetPaymentEndpoint);
        }

        private void OnPurchaseResult(bool result, string itemTittle)
        {
            NotificationPanel.instance.SetInfo($"Вы приобрели {itemTittle}");
            NotificationPanel.instance.ShowPanel();
        }
    }
}