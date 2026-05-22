

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Context;
using SC.Internship.Common.ScResult;

namespace PaymentAPI.Features.PaymentFeature;

[Authorize]

[Route("payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentTransaction _paymentTransaction;

    public PaymentController(IPaymentTransaction paymentTransaction)
    {
        _paymentTransaction = paymentTransaction;
    }


    [HttpPost("{ownerId:guid}")]
    public async Task<IActionResult> CreateTransaction([FromRoute] Guid ownerId)
    {
        var result = await _paymentTransaction.CreateTransaction(ownerId);
        return Ok(new ScResult<Guid>
        {
            Result = result
        });
    }

    [HttpPut("{id:guid}/{state}")]
    public async Task<ScResult<Guid>> ConfirmTransaction([FromRoute] Guid id, [FromRoute] string state)
    {
        
        if (state == "Confirmed")
        {
            var result = await _paymentTransaction.ConfirmTransaction(id);
            return new ScResult<Guid>
            {
                Result = result
            };
        }
        else
        {
            var result = await _paymentTransaction.CancelTransaction(id);
            return new ScResult<Guid>
            {
                Result = result
            };
        }

    }
}

  