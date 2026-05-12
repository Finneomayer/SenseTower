using System.Collections.Generic;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Mechanics.SendPurchaseSpaceRequest.Models;
using Models;
using Newtonsoft.Json;
using Proyecto26;

namespace Mechanics.SendPurchaseSpaceRequest
{
    public class SpacePurchaseService : ISpacePurchaseService
    {
        private readonly IClientData _clientData;

        public SpacePurchaseService(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<SendPurchaseResult> SendPurchaseRequest(PurchaseRequestDTO purchaseRequestDto)
        {
            var utcs = new UniTaskCompletionSource<SendPurchaseResult>();
            if (string.IsNullOrEmpty(purchaseRequestDto.PhoneNumber))
                purchaseRequestDto.PhoneNumber = null;
            
            if (string.IsNullOrEmpty(purchaseRequestDto.Comment))
                purchaseRequestDto.Comment = null;
            
            RequestHelper options = new RequestHelper();
            options.Uri = APIService.AddLanguageParameter(APIService.SendSpacePurchaseRequestUrl);
            options.BodyString = JsonConvert.SerializeObject(purchaseRequestDto);
            
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<ScResult> sendSpacePurchaseRequestResult =
                await WebRequestFunctions.PostWithDeserialization<ScResult>(options);
            if (sendSpacePurchaseRequestResult.ResponseData == null)
            {
                utcs.TrySetResult(null);
            }
            else
            {
                utcs.TrySetResult(MapToSendPurchaseResult(sendSpacePurchaseRequestResult.ResponseData,
                    sendSpacePurchaseRequestResult.ResponseCode == HttpResponse<ScResult>.SuccessCode));
            }

            return await utcs.Task;
        }
        
        private SendPurchaseResult MapToSendPurchaseResult(ScResult message, bool success = false)
        {
            SendPurchaseResult sendPurchaseResult = new();
            sendPurchaseResult.success = success;
            if (message.Error != null)
            {
                sendPurchaseResult.ValidationError = new();
                sendPurchaseResult.ValidationError.Message = message.Error.Message;

                foreach (KeyValuePair<string, List<string>> errors in message.Error.ModelState!)
                {
                    if (errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.Name))
                        || errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.InvalidUserName)))
                    {
                        sendPurchaseResult.ValidationError.Name = string.Join(",", errors.Value);
                    }

                    if (errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.PhoneNumber)))
                    {
                        sendPurchaseResult.ValidationError.PhoneNumber = string.Join(",", errors.Value);
                    }

                    if (errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.Email)))
                    {
                        sendPurchaseResult.ValidationError.Email = string.Join(",", errors.Value);
                    }
                    if (errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.Comment)))
                    {
                        sendPurchaseResult.ValidationError.Comment = string.Join(",", errors.Value);
                    }
                    if (errors.Key.Equals(nameof(sendPurchaseResult.ValidationError.Message)))
                    {
                        sendPurchaseResult.ValidationError.Space = string.Join(",", errors.Value);
                    }
                }
            }

            return sendPurchaseResult;
        }
    }
}