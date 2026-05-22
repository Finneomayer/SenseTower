namespace SC.SenseTower.Common.Services;

public interface IBaseHttpService
{
    Task<T?> Get<T>(string? token, string url, object? data, CancellationToken cancellationToken);
    Task<T?> Post<T>(string? token, string url, object? data, CancellationToken cancellationToken);
}