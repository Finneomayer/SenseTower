namespace SenseWebApi1.Services;

public interface IHttpService
{
    Task<string?> GetImages(Guid id);

    Task<string?> GetSpaces(Guid id);

    Task<HttpResponseMessage> CreateTransaction(Guid ownerId, CancellationToken cancellationToken);
    
    Task<HttpResponseMessage> ConfirmTransaction(Guid ownerId, CancellationToken cancellationToken);
    
    Task<HttpResponseMessage> CancelTransaction(Guid ownerId, CancellationToken cancellationToken);
}