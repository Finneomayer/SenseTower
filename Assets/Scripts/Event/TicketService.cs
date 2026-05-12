using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Assets.Scripts.Event
{
    public class TicketService : ITicketService
    {
        private IClientData _clientData;

        [Inject]
        public void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<bool> BuyTicket(Guid eventId, Guid userId)
        {
            await UniTask.WaitUntil(() => APIService.GetTicketsUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<bool>();

            string url = $"{APIService.GetTicketsUrl}/buy/{eventId}";
            HttpResponse<string> httpResponse = await WebRequestFunctions.Post(url, _clientData.AccessToken);

            utcs.TrySetResult(httpResponse.ResponseCode == HttpResponse<string>.SuccessCode);

            return await utcs.Task;
        }
    }
}