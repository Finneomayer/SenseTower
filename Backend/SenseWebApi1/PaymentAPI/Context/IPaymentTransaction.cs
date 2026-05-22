namespace PaymentAPI.Context;

public interface IPaymentTransaction
{
    Task<Guid> CreateTransaction(Guid ownerId);

    Task<Guid> ConfirmTransaction(Guid id);

    Task<Guid> CancelTransaction(Guid id);
}