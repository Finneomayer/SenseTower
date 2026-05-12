using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LikeService : ILikeService
{
    public async UniTask<bool> Like(Guid spaceId, bool? like, IClientData clientData)
    {
        var utcs = new UniTaskCompletionSource<bool>();

        await UniTask.WaitUntil(() => APIService.GetSpacesUrl != string.Empty);
        
        var options = new RequestHelper()
        {
            Uri = APIService.GetSpacesUrl + $"/{spaceId}/like",
        };

        ASCIIEncoding encoding = new ASCIIEncoding();

        if (!like.HasValue) options.BodyRaw = encoding.GetBytes($"{{\"like\": null}}");        
        else if (like.Value == true) options.BodyRaw = encoding.GetBytes($"{{\"like\": true}}");
        else options.BodyRaw = encoding.GetBytes($"{{\"like\": false}}");        
        options.Headers["Authorization"] = $"Bearer {clientData.AccessToken}";
        options.Headers["Content-Type"] = $"application/json";

        RestClient.Post(options).Then(response =>
        {
            utcs.TrySetResult(true);
        })
            .Catch(err =>
            {
                Debug.LogError("Like request " + err.Message);
                
                utcs.TrySetResult(false);
            });

        return await utcs.Task;
    }    
}
