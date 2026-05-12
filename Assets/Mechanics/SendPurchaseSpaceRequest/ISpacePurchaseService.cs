using Cysharp.Threading.Tasks;
using Mechanics.SendPurchaseSpaceRequest.Models;
using Models;

namespace Mechanics.SendPurchaseSpaceRequest
{
    public interface ISpacePurchaseService
    {
        public UniTask<SendPurchaseResult> SendPurchaseRequest(PurchaseRequestDTO purchaseRequestDto);
    }
}