using Cysharp.Threading.Tasks;
using Data;

namespace Assets.Scripts.Transactions
{
    public interface ITransactionsService
    {
        UniTask<TransactionDto[]> GetTransactions(int? limit = null, int? offset = null);

        UniTask<TransactionDto[]> GetTransactionsByType(Enumenators.TransactionPurposeTypeDto[] twrOperationTypes,
            int? limit = null, int? offset = null);
    }
}