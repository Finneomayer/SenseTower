using PaymentAPI.Controllers;

namespace PaymentAPI.Features.PaymentFeature;

public class PaymentTransaction
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public Guid Id { get; set; }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public PaymentEnum State { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime DateCreation { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime ?DateConfirmation { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime ?DateCancellation { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Description { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid OwnerId { get; set; }
}