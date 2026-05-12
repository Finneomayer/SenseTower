using UnityEngine;
using System;

namespace UI
{
    public sealed class TwrTransactionsFilterTabView : MonoBehaviour
    {
        [SerializeField]
        private TransactionFilterTabItem[] TabItems;

        public event Action<TransactionsFilterType> FilterClicked;

        [Serializable]
        private class TransactionFilterTabItem
        {
            public ButtonUI Button;
            public TransactionsFilterType Filter;
        }

        private void OnEnable()
        {
            foreach (var item in TabItems)
            {
                item.Button.InteractElement.onClick.AddListener(() => OnTabClick(item));
            }
        }

        private void OnDisable()
        {
            foreach (var item in TabItems)
            {
                item.Button.InteractElement.onClick.RemoveAllListeners();
            }
        }

        public void SetTabForFilter(TransactionsFilterType filter)
        {
            foreach (var item in TabItems)
            {
                item.Button.SetButtonActive(item.Filter == filter);
            }
        }

        private void OnTabClick(TransactionFilterTabItem tabItem)
        {
            FilterClicked?.Invoke(tabItem.Filter);
        }      
    }
}
