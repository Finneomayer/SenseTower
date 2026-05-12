using System;
using System.Collections.Generic;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.MyPlaces
{
    public class MyPlaceService : IMyPlaceService
    {
        public static event Action UpdateMyPlaceDataEvent; //uses for linking UI menu update image & OfficeInfrastructure image update on walls
        private IClientData _clientData;

        [Inject]
        public void Init(IClientData clientData)
        {
            _clientData = clientData;
        }

        public void UpdateMyPlaceData()
        {
            UpdateMyPlaceDataEvent?.Invoke();
        }

        public async UniTask<bool> ReplaceAllMyPlacePictures(Guid myPlaceId, Dictionary<int, MyImage> myPlaceImages)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.ReplaceAllMyPlacePicturesUrl != string.Empty);

            //WWWForm myPicturesUpdateData = new();
            //myPicturesUpdateData.AddField("SpaceId", "9fd08c9c-621c-4f36-bc73-6bebeb7d6ff6");

            var form = new Dictionary<string, string>();
            form.Add("SpaceId", myPlaceId.ToString());

            int counter = 0;
            foreach (var i in myPlaceImages)
            {
                form.Add($"Images[{counter}][location]", i.Key.ToString());
                form.Add($"Images[{counter}][imageId]", i.Value.Id.ToString());
                counter++;
            }

            var options = new RequestHelper()
            {
                Uri = APIService.ReplaceAllMyPlacePicturesUrl,
                //FormData = myPicturesUpdateData,
                SimpleForm = form,
            };

            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            RestClient.Put(options).Then(response =>
                {
                    utcs.TrySetResult(true);
                })
                .Catch(err =>
                {
                    Debug.LogError("ReplaceAllMyPlacePictures" + err.Message);
                    RequestException ex = (RequestException)err;
                    Debug.LogError(ex.Response);

                    utcs.TrySetResult(false);
                });

            return await utcs.Task;
        }

        public async UniTask<bool> UpdateDoorImage(Guid spaceId, Guid imageId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.UpdateDoorImageUrl != string.Empty);

            Debug.LogWarning(APIService.UpdateDoorImageUrl);
            var options = new RequestHelper()
            {
                Uri = $"{APIService.UpdateDoorImageUrl}/{spaceId}/doorimage/{imageId}?",
                //Uri = $"https://dev.sensetower.io/spaces/api/v1/spaces/{spaceId}/doorimage/{imageId}?",
            };
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            RestClient.Put(options).Then(response =>
                {
                    utcs.TrySetResult(true);
                })
                .Catch(err =>
                {
                    Debug.LogError("UpdateImage" + err.Message);
                    utcs.TrySetResult(false);
                });

            return await utcs.Task;


        }

        public async UniTask<bool> ResetDoorImage(Guid spaceId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.UpdateDoorImageUrl != string.Empty);

            Debug.LogWarning(APIService.UpdateDoorImageUrl);
            var options = new RequestHelper()
            {
                Uri = $"{APIService.UpdateDoorImageUrl}/{spaceId}/doorimage",
            };
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            RestClient.Put(options).Then(response =>
            {
                utcs.TrySetResult(true);
            })
            .Catch(err =>
            {
                Debug.LogError($"{nameof(ResetDoorImage)}. {err.Message}");
                utcs.TrySetResult(false);
            });

            return await utcs.Task;
        }

        public async UniTask<LocalSpace[]> GetAllMySpaces()
        {
            var utcs = new UniTaskCompletionSource<LocalSpace[]>();

            await UniTask.WaitUntil(() => APIService.GetMySpacePageUrl != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            var options = new RequestHelper()
            {
                Uri = $"{APIService.GetMySpacePageUrl}?_={DateTime.Now.Millisecond.ToString()}",
                Headers = new Dictionary<string, string>()
                {
                    {"Authorization", "Bearer " + _clientData.AccessToken}
                }
            };
            options.Uri = APIService.AddLanguageParameter(options.Uri);
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            RestClient.Get(options).Then(response =>
                {
                    utcs.TrySetResult(DeserializeData<LocalSpace[]>(response));
                })
                .Catch(err =>
                {
                    //Debug.LogWarning(err.Message);
                    utcs.TrySetResult(Array.Empty<LocalSpace>());
                });

            return await utcs.Task;
        }

        public async UniTask<bool> AddAdmin(Guid spaceId, Guid userId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.GetSpacesUrlV2 != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            string url = $"{APIService.GetSpacesUrlV2}/{spaceId}";
            PatchRequestDto patchRequestDto = new PatchRequestDto()
            {
                OperationType = 0,
                Path = "AdminIds/-",
                Op = "add",
                Value = userId.ToString()
            };

            HttpResponse<string> result = await WebRequestFunctions.Patch(url, new PatchRequestDto[] { patchRequestDto },
                _clientData.AccessToken);

            utcs.TrySetResult(result != null && result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode);

            return await utcs.Task;
        }

        public async UniTask<bool> UpdateAdminList(Guid spaceId, List<string> userIds)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.GetSpacesUrlV2 != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            string url = $"{APIService.GetSpacesUrlV2}/{spaceId}";
            PatchRequestDto patchRequestDto = new PatchRequestDto()
            {
                OperationType = 0,
                Path = "AdminIds/",
                Op = "replace",
                Value = userIds
            };

            HttpResponse<string> result = await WebRequestFunctions.Patch(url, new PatchRequestDto[] { patchRequestDto },
                _clientData.AccessToken);

            utcs.TrySetResult(result != null && result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode);

            return await utcs.Task;
        }

        public async UniTask<string[]> GetBlockList(Guid spaceId)
        {
            var utcs = new UniTaskCompletionSource<string[]>();

            await UniTask.WaitUntil(() => APIService.GetSpacesUrlV2 != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            string url = $"{APIService.GetSpacesUrlV2}/{spaceId}/blocklist";

            HttpResponse<ScResult<string[]>> httpResponse = 
                await WebRequestFunctions.GetWithDeserialization<ScResult<string[]>>(url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode &&
                httpResponse.ResponseData.Error == null && httpResponse.ResponseData.Result != null)
            {
                utcs.TrySetResult(httpResponse.ResponseData.Result);
            }
            else
            {
                utcs.TrySetResult(Array.Empty<string>());
            }

            return await utcs.Task;
        }

        public async UniTask<bool> AddToBlockList(Guid spaceId, Guid userId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.GetSpacesUrlV2 != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            string url = $"{APIService.GetSpacesUrlV2}/{spaceId}/blocklist/{userId}";

            HttpResponse<string> httpResponse =
                await WebRequestFunctions.Post(url, _clientData.AccessToken);

            if (httpResponse != null && httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> DeleteFromBlockList(Guid spaceId, Guid userId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.GetSpacesUrlV2 != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            string url = $"{APIService.GetSpacesUrlV2}/{spaceId}/blocklist/{userId}";

            HttpResponse<string> httpResponse =
                await WebRequestFunctions.Delete(url, _clientData.AccessToken);

            if (httpResponse != null && httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
            }

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
                Debug.Log($"<color=red>Error deserialize {typeof(T)}: </color>" + e);
            }

            return deserialized;
        }
    }
}