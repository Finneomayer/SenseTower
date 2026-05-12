using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.InAppPurchase.UI
{
    public class AppPurchaseViewElement : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private TMP_Text _tittle;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _buyButton;
        #endregion

        private void OnDisable()
        {
            if (_buyButton != null)
                _buyButton.onClick.RemoveAllListeners();
        }

        public void SetInfo(string tittle)
        {
            _tittle.text = tittle;
        }

        public void SubscribeToBuyButtonClick(Action<string> onBuyButtonClickEvent)
        {
            if (_buyButton != null)
                _buyButton.onClick.AddListener(() => onBuyButtonClickEvent.Invoke(_tittle.text));
        }
    }
}