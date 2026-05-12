using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using System;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Hall;
using Zenject;

public class MyImageService : IMyImageService
{
    private IClientData _clientData;

    [Inject]
    public void Init(IClientData clientData)
    {
        _clientData = clientData;
    }

    public async UniTask<MyImage[]> GetAllImages()
    {
        var utcs = new UniTaskCompletionSource<MyImage[]>();
        Debug.Log(APIService.GetMyImagePageUrl);
        var options = new RequestHelper()
        {
            
            Uri = $"{APIService.GetMyImagePageUrl}?_={DateTime.Now.Millisecond.ToString()}",
            Headers = new Dictionary<string, string>()
            {
                {"Authorization", "Bearer " + _clientData.AccessToken}
            }
        };

        options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

        RestClient.Get(options).Then(response => 
            {
                utcs.TrySetResult(DeserializeData<MyImage[]>(response));
            })
            .Catch(err =>
            {
                Debug.LogWarning(err.Message);
                utcs.TrySetResult(null);
            });

        return await utcs.Task;
    }

    private T DeserializeData<T>(ResponseHelper response) where T : class
    {
        T deserialized = null;
        try
        {
            deserialized = JsonConvert.DeserializeObject<T>(response.Text);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"<color=red>Error deserialize {typeof(T)}: </color>" + e);
        }

        return deserialized;
    }
}
