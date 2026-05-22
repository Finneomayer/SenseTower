using PaymentAPI.Controllers;
using PaymentAPI.Features.PaymentFeature;
using SC.Internship.Common.Exceptions;

namespace PaymentAPI.Context;

public class PaymentTransactionContext:IPaymentTransaction
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once CollectionNeverQueried.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private  List<PaymentTransaction> _transactions = new();
    public async Task<Guid> CreateTransaction(Guid ownerId)
    {
        try
        {
            var transaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                DateCreation = DateTime.Now,
                DateCancellation = null,
                DateConfirmation = null,
                Description = null,
                OwnerId = ownerId,
                State = PaymentEnum.Hold
            };
            await Task.Run(()=>_transactions.Add(transaction));
            
            return transaction.Id;
        }
        catch
        {
            throw new ScException("Не удалось создать транзакцию");
        }
        
    }

    public async Task<Guid> ConfirmTransaction(Guid id)
    {
        try
        {
            
            var transaction = _transactions.Find(p => p.Id == id);
            if (transaction != null)
            {
                transaction.DateConfirmation=DateTime.Now;
                transaction.State = PaymentEnum.Confirmed;
            }
            else
            {
                throw new ScException("Транзакция не найдена");
            }
            await Task.CompletedTask;
            return transaction.Id;
        }
        catch
        {
            throw new ScException("Не удалось подтвердить транзакцию");
        }
    }

    public async Task<Guid> CancelTransaction(Guid id)
    {
        try
        {
            var transaction = _transactions.Find(p => p.Id == id);
            if (transaction != null)
            {
                transaction.DateCancellation=DateTime.Now;
                transaction.State = PaymentEnum.Canceled;
            }
            else
            {
                throw new ScException("Транзакция не найдена");
            }
            
            await Task.CompletedTask;
            
            return transaction.Id;
        }
        catch
        {
            throw new ScException("Не удалось отменить транзакцию");
        }
    }
}