using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Transactions;
using Data;
using System;
using System.Threading;

namespace UI
{
    public enum TransactionsFilterType
    {
        MiningBurning = 0,
        BuySell = 1,
        Transfer = 2,
        SpaceEntry = 3,
    }

    public sealed class TwrTransactionsViewPanel : ViewPanel
    {
        [SerializeField]
        private TwrTransactionsView TransactionView;
        [SerializeField]
        private TwrTransactionsFilterTabView TransactionsFilterTabView;
        [SerializeField]
        private GameObject LoadingPanel;

        private ITransactionsService _transactionsService;
        private TransactionsFilterType _currentTransactionFilterType = TransactionsFilterType.BuySell;
        private CancellationTokenSource _cancelTokenSource;

        [Inject]
        public void Construct(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        private void OnEnable()
        {
            TransactionsFilterTabView.FilterClicked += OnTransactionsFilterClicked;
            TransactionView.TransactionsRequested += OnTransactionViewTransactionsRequested;
        }

        private void OnDisable()
        {
            TransactionsFilterTabView.FilterClicked -= OnTransactionsFilterClicked;
            TransactionView.TransactionsRequested -= OnTransactionViewTransactionsRequested;
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            LoadingPanel.SetActive(false);
            TransactionsFilterTabView.SetTabForFilter(_currentTransactionFilterType);
            TransactionView.Show();
        }

        public override void HidePanel()
        {
            TransactionView.Hide();
            base.HidePanel();
        }

        private Enumenators.TransactionPurposeTypeDto[] GetTransactionTypesByFilter(TransactionsFilterType filterType)
        {
            Enumenators.TransactionPurposeTypeDto[] purposeTypes;
            switch (filterType)
            {
                case TransactionsFilterType.MiningBurning:
                    purposeTypes = new Enumenators.TransactionPurposeTypeDto[] {
                        Enumenators.TransactionPurposeTypeDto.TimeMining,
                        Enumenators.TransactionPurposeTypeDto.TimeBurning 
                    };
                    break;
                case TransactionsFilterType.BuySell:
                    purposeTypes = new Enumenators.TransactionPurposeTypeDto[] { Enumenators.TransactionPurposeTypeDto.BuySell };
                    break;
                case TransactionsFilterType.Transfer:
                    purposeTypes = new Enumenators.TransactionPurposeTypeDto[] { Enumenators.TransactionPurposeTypeDto.Transfer };
                    break;
                case TransactionsFilterType.SpaceEntry:
                    purposeTypes = new Enumenators.TransactionPurposeTypeDto[] { Enumenators.TransactionPurposeTypeDto.SpaceEntryFee };
                    break;
                default:
                    purposeTypes = Array.Empty<Enumenators.TransactionPurposeTypeDto>();
                    break;
            }

            return purposeTypes;
        }

        private void OnTransactionsFilterClicked(TransactionsFilterType filter)
        {
            _currentTransactionFilterType = filter;
            TransactionsFilterTabView.SetTabForFilter(_currentTransactionFilterType);
            if (IsVisible())
            {
                TransactionView.Show(resetPagination: true);
            }
        }

        private async UniTask LoadTransactions(int offset, int limit, CancellationToken cancelationToken)
        {
            LoadingPanel.SetActive(true);

            TransactionDto[] transactions = await _transactionsService.GetTransactionsByType(
                GetTransactionTypesByFilter(_currentTransactionFilterType), limit, offset);

            if (cancelationToken.IsCancellationRequested)
            {
                return;
            }
            TransactionView.FillTransactions(transactions);

            LoadingPanel.SetActive(false);
        }

        private void OnTransactionViewTransactionsRequested(int offset, int limit)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource = new CancellationTokenSource();

            LoadTransactions(offset, limit, _cancelTokenSource.Token).Forget();
        }
    }
}
