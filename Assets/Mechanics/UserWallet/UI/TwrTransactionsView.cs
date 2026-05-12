using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class TwrTransactionsView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect TransactionScrollRect;
        [SerializeField]
        private RectTransform TransactionViewItemsContent;
        [SerializeField]
        private TwrTransactionItem TransactionViewItemPrefab;
        [SerializeField]
        private int ShowingItemsScrollIncrement = 20;

        private int _currentMaxShowingCount = 0;
        private List<TwrTransactionItem> _transactionViewItems = new();
        private List<TransactionDto> _loadedTransactions = new();
        private bool _waitingForTransactions;

        public event Action<int, int> TransactionsRequested;

        public void Show(bool resetPagination = false)
        {
            DestroyItems();

            if (resetPagination)
            {
                _currentMaxShowingCount = ShowingItemsScrollIncrement;
                TransactionScrollRect.normalizedPosition = Vector2.one;
            }

            if (_currentMaxShowingCount < ShowingItemsScrollIncrement)
            {
                _currentMaxShowingCount = ShowingItemsScrollIncrement;
            }

            RegisterListeners();

            _loadedTransactions.Clear();

            RequestTransactions(_currentMaxShowingCount);
        }

        private void RequestTransactions(int limit)
        {
            _waitingForTransactions = true;
            TransactionsRequested?.Invoke(0, limit);
        }

        public void FillTransactions(TransactionDto[] transactions)
        {
            _waitingForTransactions = false;
            _loadedTransactions = transactions.ToList();

            DestroyItems();
            if (_loadedTransactions == null)
            {
                return;
            }

            for (int i = 0; i < _loadedTransactions.Count; i++)
            {
                _transactionViewItems.Add(CreateItem(_loadedTransactions[i]));
            }
            if (_currentMaxShowingCount < _loadedTransactions.Count)
            {
                _currentMaxShowingCount = _loadedTransactions.Count;
            }
        }

        public void Hide()
        {
            DestroyItems();
            UnregisterListeners();
        }

        private void RegisterListeners()
        {
            UnregisterListeners();
            TransactionScrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void UnregisterListeners()
        {
            TransactionScrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        private void OnScroll(Vector2 scrollPosition)
        {
            if (_waitingForTransactions)
            {
                return;
            }

            if (scrollPosition.y <= 0.1f)
            {
                if (_currentMaxShowingCount <= _loadedTransactions.Count)
                {
                    _currentMaxShowingCount += ShowingItemsScrollIncrement;
                    RequestTransactions(_currentMaxShowingCount);
                }
            }
        }

        private TwrTransactionItem CreateItem(TransactionDto transactionData)
        {
            TwrTransactionItem towerEventView = Instantiate(TransactionViewItemPrefab, TransactionViewItemsContent);
            towerEventView.Init(transactionData);

            return towerEventView;
        }

        private void DestroyItems()
        {
            if (_transactionViewItems == null)
            {
                return;
            }
            foreach (var item in _transactionViewItems)
            {
                Destroy(item.gameObject);
            }
            _transactionViewItems.Clear();
        }
    }
}
