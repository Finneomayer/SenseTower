using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Data;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.TowerObjects
{
    public class TowerObjectsService : ITowerObjectsService
    {
        private IClientData _clientData;

        [Inject]
        public void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<TowerObjectDto[]> GetTowerObjects()
        {
            return await RequestTowerObjects();
        }

        public async UniTask<TowerObjectDto[]> GetUserObjects()
        {
            await UniTask.WaitUntil(() => _clientData != null && _clientData.UserId != null);
            return await RequestTowerObjects(_clientData.UserId);
        }

        private List<TowerObjectDto> GetStubObjects()
        {
            List<TowerObjectDto> towerObjectsList = new();

            for (int i = 0; i < 3; i++)
            {
                TowerObjectDto cameraObject = new()
                {
                    Id = new Guid($"85a27436-a834-42f9-9244-2aa9e95be51{i}"),
                    ObjectClassId = new Guid("85a27436-a834-42f9-9244-2aa9e95be500"),

                    Name = $"Camera {i + 1}",
                    Description = "Broadcast camera",
                    TowerObjectClassName = "BroadcastTranslationCamera",

                    //BehaviorType = TowerObjectBehaviorType.Movable,
                    PrefabObjectType = Enumenators.PrefabObjectType.Camera,

                    RemoteObjectTypeInfo = null,

                    OwnerId = null,
                    OwnerName = "Магазин",
                    OwnerBusinessUnitType = 0,
                };
                towerObjectsList.Add(cameraObject);
            }

            for (int i = 0; i < 3; i++)
            {
                TowerObjectDto padObject = new()
                {
                    Id = new Guid($"85a27436-a834-42f9-9244-2aa9e95be52{i}"),
                    ObjectClassId = new Guid("85a27436-a834-42f9-9244-2aa9e95be501"),

                    Name = $"Pad {i + 1}",
                    Description = "Pad",
                    TowerObjectClassName = "Personal pad",

                    //BehaviorType = TowerObjectBehaviorType.Movable,
                    PrefabObjectType = Enumenators.PrefabObjectType.Tablet,

                    RemoteObjectTypeInfo = null,

                    OwnerId = null,
                    OwnerName = "Магазин",
                    OwnerBusinessUnitType = 0,
                };
                towerObjectsList.Add(padObject);
            }

            for (int i = 0; i < 3; i++)
            {
                TowerObjectDto mirrorObject = new()
                {
                    Id = new Guid($"85a27436-a834-42f9-9244-2aa9e95be53{i}"),
                    ObjectClassId = new Guid("85a27436-a834-42f9-9244-2aa9e95be502"),

                    Name = $"Mirror {i + 1}",
                    Description = "Mirror",
                    TowerObjectClassName = "Personal Mirror",

                    //BehaviorType = TowerObjectBehaviorType.Movable,
                    PrefabObjectType = Enumenators.PrefabObjectType.Mirror,

                    RemoteObjectTypeInfo = null,

                    OwnerId = null,
                    OwnerName = "Магазин",
                    OwnerBusinessUnitType = 0,
                };
                towerObjectsList.Add(mirrorObject);
            }

            return towerObjectsList;
        }

        private async UniTask<TowerObjectDto[]> RequestTowerObjects(Guid? ownerId = null)
        {
            var utcs = new UniTaskCompletionSource<TowerObjectDto[]>();

            await UniTask.WaitUntil(() => APIService.GetTowerObjectsUrl != null);

            string url = APIService.GetTowerObjectsUrl;
            if (ownerId != null)
            {
                url = $"{url}?ownerId={ownerId}";
            }
            url = APIService.AddLanguageParameter(url);

            HttpResponse<RequestDto<TowerObjectDto[]>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<RequestDto<TowerObjectDto[]>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(Array.Empty<TowerObjectDto>());
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetTowerObjects)}. Cannot get TowerObjects. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(Array.Empty<TowerObjectDto>());
            }

            return await utcs.Task;
        }
    }
}