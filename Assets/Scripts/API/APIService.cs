using System;
using System.Linq;
using System.Collections.Generic;
using API.Models;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;
using Assets.Mechanics.MetaAvatars.Scripts;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;
using Data;
using Infrastructure.AssetManagement;
using Assets.Localization;

namespace Assets.Scripts.API
{
    public class APIService : IApiService
    {
        #region Const

        private const string BaseDevelopUrl = "http://51.250.89.118:7784/";
        private const string BaseDemoUrl = "https://demo.sensetower.io/vr-discovery/";
        private const string BaseTestUrl = "https://test.sensetower.io/vr-discovery/";
        private const string BaseProductionUrl = "https://api.sensetower.io/vr-discovery/";

        public static string AuthUrl = string.Empty;
        public static string RegistrationPageUrl = string.Empty;
        public static string RegistrationUrl = string.Empty; //https://dev.sensetower.io/accounts/api/v1/accounts/identity/register
        public static string RegistrationAsGuestUrl = string.Empty;
        public static string MakeGuestResidentUrl = string.Empty; //https://dev.sensetower.io/api/Accounts/{userId}/MakeGuestResident
        public static string ResetPasswordPageUrl = string.Empty;
        public static string RefreshTokenUrl = string.Empty;
        public static string GetUserInfoUrl = string.Empty; //"https://dev.sensetower.io/accounts/api/v2/accounts/userInfo";

        public static string GetMyImagePageUrl = string.Empty;

        //public static string GetMyPlacePageUrl = string.Empty;
        public static string GetMySpacePageUrl = string.Empty; //https://dev.sensetower.io/spaces/api/v1/spaces/owned
        public static string UpdateMyPlaceUrl = string.Empty;
        public static string UsersInSpacesUrl = string.Empty;
        public static string GetSpacesUrl = string.Empty;
        public static string GetSpacesUrlV2 = string.Empty; //"https://dev.sensetower.io/spaces/api/v2/spaces";
        public static string SetAvatarUrl = string.Empty;
        public static string GetAllUsersUrl = string.Empty; //"https://test.sensetower.io/accounts/api/v1/accounts/userInfo/Lookup";
        public static string SetAvatarWatchUrl = string.Empty; //https://dev.sensetower.io/accounts/api/v1/accounts/userinfo/setavatarwatch

        public static string SetOculusAvatarWatchUrl = string.Empty; //https://dev.sensetower.io/accounts/api/v1/accounts/userinfo/setoculusavatarwatch

        public static string GetTowerEventsUrl = string.Empty;
        public static string GetTowerNewsUrl = string.Empty;
        public static string RegistrationInSpacesUrl = "https://dev.sensetower.io/api/v1/SessionAuthorities/RegisterUserInSpace";
        public static string CheckUserAccessToSpacesUrl = "https://dev.sensetower.io/api/v1/SessionAuthorities/UsersAccess";
        public static string GetHallsUrl = string.Empty;
        public static string GetGalleryUrl = string.Empty;
        public static string GetRemoteContentLocationEndPoint = string.Empty;
        public static string GetRemoteSceneObjectLocationEndPoint = string.Empty;
        public static string GetBroadcastingServiceEndPoint = string.Empty;

        public static string ReplaceAllMyPlacePicturesUrl = string.Empty; //"https://dev.sensetower.io/spaces/api/v1/spaces/images/replaceall"; OLD version: "https://dev.sensetower.io/myplaces/api/v1/myplaces/images/replaceall";

        //public static string GetMyPlaceBySpaceUrl = string.Empty; //"https://dev.sensetower.io/myplaces/api/v1/myplaces/user/placebyspace";
        //public static string GetMyPlaceUrl = string.Empty; //"https://dev.sensetower.io/myplaces/api/v1/myplaces/user/place";
        public static string UpdateDoorImageUrl = string.Empty; //https://dev.sensetower.io/spaces/api/v1/spaces //OLD version: "https://dev.sensetower.io/myplaces/api/v1/myplaces/user/updateimage";

        public static string GetTicketsUrl = string.Empty; //"https://dev.sensetower.io/tickets/api/v1/tickets";
        public static string GetCinemasUrl = string.Empty; //"https://dev.sensetower.io/cinemas/api/v1/cinemas";
        public static string GetScreenSharingUrl = string.Empty;
        public static string GetBroadcastingKey = string.Empty;
        public static string AppVersion = string.Empty;
        public static string MetricaAppKey = string.Empty; //b2914d65-f80d-4eff-8669-9cc76aefeba9
        public static int BundleVersion = 0;
        public static string GetWalletUrl = string.Empty;
        public static string SpaceAccessPaymentsUrl = string.Empty;
        public static string SetPaymentSpaceAccessTypeUrl = string.Empty;
        public static string SendSpacePurchaseRequestUrl = string.Empty;
        public static string WhoInsideAccessTypeUrl = string.Empty;
        public static string LeftSideHallAdvertisementBilboardEndPoint;
        public static string RightSideHallAdvertisementBilboardEndPoint;
        public static string GetTransactionsUrl = string.Empty;
        public static string GetTowerObjectsUrl = string.Empty;
        public static string GetSpaceStaticObjectsEndPoint = string.Empty;
        public static string GetSpaceObjectsNewEndPoint = string.Empty; //https://dev.sensetower.io/api/spaces/{spaceId}?includes=Objects
        public static string GetTowerObjectsClassEndPoint = string.Empty;
        public static string GetTowerObjectsRevisionEndPoint = string.Empty;
        public static string GetTipsEndPoint = string.Empty;
        public static string GetShopsUrl = string.Empty;
        public static string FriendsUrl = string.Empty;
        public static string GetBuySellContractsUrl = string.Empty;
        public static string GetPaymentEndpoint = string.Empty;
        public static string GetMafiaEndPoint = string.Empty;
        public static string GetUserHoursEndPoint = string.Empty; //https://dev.sensetower.io/api/Accounts/{userId}/hours
        public static string GetUserSellerStatus = string.Empty; //https://dev.sensetower.io/api/Accounts/{userId}/is-seller
        public static string GetUserFullFledgedStatusEndPoint = string.Empty; //https://dev.sensetower.io/api/Accounts/{userId}/is-full-fledged-user
        public static string GetUserInitialBonusEndPoint = string.Empty; //https://dev.sensetower.io/api/Accounts/{userId}/initial-bonus
        public static string GetDateWhenInitialBonusStarted = string.Empty; //https://dev.sensetower.io/api/Accounts/date-when-initial-bonus-started

        /// Server URLs

        #endregion

        #region Events

        public event Action ServerInitializedSuccess;

        public event Action FailServerInitialize;
        public event Action AuthSuccess;

        #endregion

        private IClientData _clientData;
        private IServerApiData _serverData;

        #region PublicMethods

        [Inject]
        public void Construct(IClientData clientData, IServerApiData serverData, ISpaceService spaceService)
        {
            _serverData = serverData;
            _clientData = clientData;
        }

        public static string AddParameter(string url, string param)
        {
            string paramString;
            if (url.Contains('?'))
            {
                paramString = $"&{param}";
            }
            else
            {
                paramString = $"?{param}";
            }

            if (url[^1].Equals('/'))
            {
                url = url.SkipLast(1).ToString();
            }

            return $"{url}{paramString}";
        }

        public static string AddLanguageParameter(string url)
        {
            if (string.IsNullOrEmpty(LocalizationManager.CurrentLanguageCode))
            {
                return url;
            }

            return AddParameter(url, $"lang={LocalizationManager.CurrentLanguageCode}");
        }

        public async UniTask<bool> SetAvatar(Guid UserId, int? avatarNumber)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            if (_clientData == null || !_clientData.UserId.HasValue)
            {
                Debug.LogWarning($"{typeof(APIService).Name}. {nameof(SetAvatar)}. " +
                                 $"ClientData.UserId == null. Cannot set avatar.");
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            string avatarIdFormValue = avatarNumber.HasValue ? avatarNumber.Value.ToString() : null;

            WWWForm myPlaceUpdateData = new();
            myPlaceUpdateData.AddField("UserId", _clientData.UserId.ToString());
            myPlaceUpdateData.AddField("avatarNumber", avatarIdFormValue);

            var options = new RequestHelper
            {
                Uri = SetAvatarUrl,
                FormData = myPlaceUpdateData,
            };
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<bool> httpResponse = await PostBool(options);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                SetClientAvatar(avatarNumber);
            }
            else
            {
                Debug.LogWarning($"{nameof(SetAvatar)}. Cannot set user avatar. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
            }

            utcs.TrySetResult(success);
            return await utcs.Task;
        }

        public async UniTask Initialize(Assembly assembly)
        {
            string URI = assembly.AssemblyType switch
            {
                Enumenators.Server.Profile.Demo => BaseDemoUrl,
                Enumenators.Server.Profile.Production => BaseProductionUrl,
                Enumenators.Server.Profile.Test => BaseTestUrl,
                _ => BaseDevelopUrl
            };

            VREnvironmentInfo environmentInfo = await Get<VREnvironmentInfo>(URI);

            if (environmentInfo == null)
            {
                FailServerInitialize?.Invoke();
                return;
            }

            _clientData.AssemblyType = assembly.AssemblyType.ToString();
            AuthUrl = environmentInfo.GetTokenEndpoint;
            RegistrationPageUrl = environmentInfo.RegistrationPageEndpoint;
            RegistrationUrl = environmentInfo.RegistrationEndpoint;
            RegistrationAsGuestUrl = environmentInfo.RegistrationAsGuestEndPoint;
            MakeGuestResidentUrl = environmentInfo.MakeGuestResidentEndpoint;
            ResetPasswordPageUrl = environmentInfo.ResetPasswordPageEndPoint;
            RefreshTokenUrl = environmentInfo.RefreshTokenPageEndPoint;
            GetMyImagePageUrl = environmentInfo.GetImagePageEndPoint;
            //GetMyPlacePageUrl = environmentInfo.GetMyPlacePageEndPoint;
            GetMySpacePageUrl = environmentInfo.GetMySpacePageEndPoint;
            UpdateMyPlaceUrl = environmentInfo.UpdateMyPlaceEndPoint;
            UsersInSpacesUrl = environmentInfo.UsersInSpacesEndPoint;
            GetSpacesUrl = environmentInfo.GetSpacesUrlEndPoint;
            GetSpacesUrlV2 = environmentInfo.GetSpacesUrlEndPointV2;
            SetAvatarUrl = environmentInfo.SetAvatarEndPoint;
            SetAvatarWatchUrl = environmentInfo.SetAvatarWatchEndPoint;
            SetOculusAvatarWatchUrl = environmentInfo.SetOculusAvatarWatchEndPoint;
            GetAllUsersUrl = environmentInfo.GetAllUsersEndPoint;
            GetTowerEventsUrl = environmentInfo.GetTowerEventsUrlEndPoint;
            GetTowerNewsUrl = environmentInfo.GetTowerNewsUrlEndPoint;
            RegistrationInSpacesUrl = environmentInfo.RegistrationInSpacesEndPoint;
            CheckUserAccessToSpacesUrl = environmentInfo.CheckUserAccessToSpacesEndPoint;
            GetHallsUrl = environmentInfo.GetHallsEndPoint;
            GetGalleryUrl = environmentInfo.GetGalleryEndPoint;
            GetRemoteContentLocationEndPoint = environmentInfo.GetRemoteContentLocationEndPoint;
            GetRemoteSceneObjectLocationEndPoint = environmentInfo.GetRemoteSceneObjectLocationEndPoint;
            ResourcesLocation.SetRemoteBasePath(GetRemoteContentLocationEndPoint);
            ReplaceAllMyPlacePicturesUrl = environmentInfo.ReplaceAllMyPlacePicturesUrl;
            //GetMyPlaceBySpaceUrl = environmentInfo.GetMyPlaceBySpaceUrl;
            //GetMyPlaceUrl = environmentInfo.GetMyPlaceUrl;
            UpdateDoorImageUrl = environmentInfo.UpdateDoorImageUrl;
            GetTicketsUrl = environmentInfo.GetTicketsEndPoint;
            GetCinemasUrl = environmentInfo.GetCinemasEndPoint;
            GetScreenSharingUrl = environmentInfo.GetScreenSharingEndPoint;
            GetBroadcastingServiceEndPoint = environmentInfo.GetBroadcastingServiceEndPoint;
            AppVersion = environmentInfo.ReleaseVersion;
            BundleVersion = environmentInfo.Bundle;
            MetricaAppKey = environmentInfo.MetricaAppKey;
            GetWalletUrl = environmentInfo.GetWalletEndPoint;
            GetTransactionsUrl = environmentInfo.GetTransactionsEndPoint;
            GetTowerObjectsUrl = environmentInfo.GetTowerObjectsEndPoint;
            GetShopsUrl = environmentInfo.GetShopsEndPoint;
            FriendsUrl = environmentInfo.FriendsEndPoint;
            GetBuySellContractsUrl = environmentInfo.GetBuySellContractsEndPoint;
            GetBroadcastingKey = environmentInfo.GetBroadcastingKey;
            LeftSideHallAdvertisementBilboardEndPoint = environmentInfo.LeftSideHallAdvertisementBilboardEndPoint;
            RightSideHallAdvertisementBilboardEndPoint = environmentInfo.RightSideHallAdvertisementBilboardEndPoint;
            GetSpaceStaticObjectsEndPoint = environmentInfo.GetSpaceStaticObjectsEndPoint;
            GetSpaceObjectsNewEndPoint = environmentInfo.GetSpaceObjectsNewEndPoint;
            GetTowerObjectsClassEndPoint = environmentInfo.GetTowerObjectsClassEndPoint;
            GetTowerObjectsRevisionEndPoint = environmentInfo.GetTowerObjectsRevisionEndPoint;
            SpaceAccessPaymentsUrl = environmentInfo.SpaceAccessPaymentsEndPoint;
            SetPaymentSpaceAccessTypeUrl = environmentInfo.SetPaymentSpaceAccessTypeEndPoint;
            SendSpacePurchaseRequestUrl = environmentInfo.SendSpacePurchaseRequestEndPoint;
            WhoInsideAccessTypeUrl = environmentInfo.SetWhoInsideAccessTypeEndPoint;
            GetTipsEndPoint = environmentInfo.GetTipsEndPoint;
            GetUserInfoUrl = environmentInfo.GetUserInfoEndPoint;
            GetPaymentEndpoint = environmentInfo.GetPaymentEndpoint;
            GetMafiaEndPoint = environmentInfo.GetMafiaEndPoint;
            GetUserHoursEndPoint = environmentInfo.GetUserHoursEndPoint;
            GetUserSellerStatus = environmentInfo.GetUserSellerStatus;
            GetUserFullFledgedStatusEndPoint = environmentInfo.GetUserFullFledgedStatusEndPoint;
            GetUserInitialBonusEndPoint = environmentInfo.GetUserInitialBonusEndPoint;
            GetDateWhenInitialBonusStarted = environmentInfo.GetDateWhenInitialBonusStarted;


        ServerInitializedSuccess?.Invoke();
        }

        public async UniTask<bool> Auth(WWWForm data)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var url = AuthUrl;
            var result = await Post<GetTokenResponse>(url, data);

            if (ReferenceEquals(result, null))
            {
                utcs.TrySetResult(false);
            }
            else
            {
                SetClientData(result);
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> AuthAsGuest(WWWForm data)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var url = AuthUrl;
            var result = await Post<GetTokenResponse>(url, data);

            if (ReferenceEquals(result, null))
            {
                utcs.TrySetResult(false);
            }
            else
            {
                result.IsGuest = true; //TODO: remove when bakend will return this value
                SetClientData(result);
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> GetHall(WWWForm data)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var url = AuthUrl;
            var result = await Post<GetTokenResponse>(url, data);

            if (ReferenceEquals(result, null))
                utcs.TrySetResult(false);
            else
                utcs.TrySetResult(!string.IsNullOrEmpty(result.AccessToken));

            SetClientData(result);

            return await utcs.Task;
        }

        public async UniTask<bool> RefreshToken()
        {
            await UniTask.WaitUntil(() => RefreshTokenUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<bool>();
            if (_clientData.IsRefreshing)
            {
                await UniTask.WaitWhile(() => _clientData.IsRefreshing);
            }

            string url = RefreshTokenUrl;

            string refreshToken = _clientData.RefreshToken;

            if (string.IsNullOrEmpty(refreshToken))
            {
                Debug.Log($"<color=red>RefreshToken is empty</color>");
                utcs.TrySetResult(false);
            }
            else
            {
                _clientData.IsRefreshing = true;

                var form = new WWWForm();
                form.AddField("refreshToken", refreshToken);
                var TokenResponse = await Post<RefreshTokenResponse>(url, form);
                if (TokenResponse == null)
                {
                    Debug.LogWarning($"<color=red>Cannot refresh token</color>");
                    utcs.TrySetResult(false);
                }
                else
                {
                    SetTokenInfo(TokenResponse.accessToken, TokenResponse.refreshToken);
                    await RefreshUserInfo();

                    utcs.TrySetResult(true);
                }

                _clientData.IsRefreshing = false;
            }

            return await utcs.Task;
        }

        public async UniTask RefreshUserInfo()
        {
            await UniTask.WaitUntil(() => GetUserInfoUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<UserInfoDto>();

            UserInfoDto userInfo = await GetWithAuthToken<UserInfoDto>(GetUserInfoUrl);

            if (userInfo != null)
            {
                _clientData.UserId = userInfo.UserId; 
                _clientData.UserName = userInfo.Login;
                _clientData.IsGuest = string.IsNullOrEmpty(userInfo.Email);
                _clientData.CreatedOn = userInfo.CreatedOn; 
                SetClientAvatar(userInfo.AvatarNumber);
                SetClientWatch(userInfo.AvatarWatchNumber, userInfo.OculusAvatarWatchNumber);
            }
        }

        public async UniTask<bool> UpdatePlaceAccessType(Guid myPlaceId, SpaceAccessType accessType, decimal tax = default)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            RequestHelper options = new RequestHelper();
            if (accessType == SpaceAccessType.Paid)
            {
                SetPaymentAccessTypeMode paymentAccessTypeMode = new();
                paymentAccessTypeMode.DailyTax = tax;
                options.Uri = $"{SetPaymentSpaceAccessTypeUrl.Replace($"{{0:spaceid}}",myPlaceId.ToString())}";
                options.BodyString = JsonConvert.SerializeObject(paymentAccessTypeMode);
            }
            else if (accessType == SpaceAccessType.WhoInside)
            {
                options.Uri = $"{WhoInsideAccessTypeUrl.Replace($"{{0:spaceid}}",myPlaceId.ToString())}";
            }
            else
            {
                WWWForm myPlaceUpdateData = new();
                myPlaceUpdateData.AddField("Id", myPlaceId.ToString());
                myPlaceUpdateData.AddField("PublicAccessType", (int) accessType);

                options = new RequestHelper
                {
                    Uri = UpdateMyPlaceUrl,
                    FormData = myPlaceUpdateData,
                };
            }

            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<EmptyResponseData> httpResponse = await Post<EmptyResponseData>(options);
            utcs.TrySetResult(httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode);

            return await utcs.Task;
        }

        public bool CheckUserAuthentication()
        {
            return !string.IsNullOrEmpty(_clientData.AccessToken);
        }

        public async UniTask<bool> PostWithAuthToken(string Url)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            if (_clientData.IsRefreshing)
            {
                await UniTask.WaitWhile(() => _clientData.IsRefreshing);
            }

            var options = new RequestHelper
            {
                Uri = Url
            };
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
            options.Headers.Add("Accept-Encoding","gzip");

            HttpResponse<bool> httpResponse = await PostBool(options);
            if (httpResponse.ResponseCode == HttpResponse<bool>.NotAuthorizedCode)
            {
                bool successAuthorization = await RefreshToken();
                if (successAuthorization)
                {
                    options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
                    httpResponse = await PostBool(options);
                }
            }

            utcs.TrySetResult(httpResponse.ResponseData);

            return await utcs.Task;
        }

        public async UniTask<T> GetWithAuthToken<T>(string Url) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                Uri = Url
            };
#if UNITY_SERVER
            if(!string.IsNullOrEmpty(_serverData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_serverData.AccessToken}";

            RestClient.Get(options).Then(response =>
                {
                    utcs.TrySetResult(DeserializeData<T>(response));
                })
                .Catch(err =>
                {
                    utcs.TrySetResult(null);
                });
#endif

#if !UNITY_SERVER
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<T> httpResponse = await Get<T>(options);
            if (httpResponse.ResponseCode == HttpResponse<T>.NotAuthorizedCode)
            {
                bool successAuthorization = await RefreshToken();
                if (successAuthorization)
                {
                    options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
                    httpResponse = await Get<T>(options);
                }
            }

            utcs.TrySetResult(httpResponse.ResponseData);
#endif

            return await utcs.Task;
        }

        public async UniTask<T> PostWithAuthToken<T>(string Url) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                Uri = Url
            };

            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
           
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            HttpResponse<T> httpResponse = await Post<T>(options);
            if (httpResponse.ResponseCode == HttpResponse<T>.NotAuthorizedCode)
            {
                bool successAuthorization = await RefreshToken();
                if (successAuthorization)
                {
                    options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
                    httpResponse = await Post<T>(options);
                }
            }

            utcs.TrySetResult(httpResponse.ResponseData);

            return await utcs.Task;
        }

        #endregion

        #region PrivateMethods

        public async UniTask<T> Get<T>(string Url) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                Uri = Url
            };
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Get(options).Then(response => { utcs.TrySetResult(DeserializeData<T>(response)); })
                .Catch(err =>
                {
                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Get)}. {err.Message}. Url: {options.Uri}");
                    utcs.TrySetResult(null);
                });

            return await utcs.Task;
        }

        private async UniTask<HttpResponse<T>> Get<T>(RequestHelper options) where T : class
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Get(options).Then(response =>
                {
                    httpResponse = new HttpResponse<T>(DeserializeData<T>(response), response.StatusCode);
                    utcs.TrySetResult(httpResponse);
                })
                .Catch(err =>
                {
                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Get)}. {err.Message}. Url: {options.Uri}");
                    httpResponse = new HttpResponse<T>(default,
                        (err as RequestException).StatusCode);
                    utcs.TrySetResult(httpResponse);
                });

            return await utcs.Task;
        }

        private async UniTask<T> Post<T, TPayLoad>(string Url, TPayLoad payLoad) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                Uri = Url,
                Body = payLoad
            };
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Post(options).Then(response => { utcs.TrySetResult(DeserializeData<T>(response)); })
                .Catch(err =>
                {
                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Get)}. {err.Message}. Url: {options.Uri}");
                    utcs.TrySetResult(null);
                });

            return await utcs.Task;
        }

        private async UniTask<T> Post<T>(string Url, WWWForm data) where T : class
        {
            var utcs = new UniTaskCompletionSource<T>();

            var options = new RequestHelper
            {
                FormData = data,
                Uri = Url
            };
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

#if UNITY_SERVER
            if(!string.IsNullOrEmpty(_serverData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_serverData.AccessToken}";
#endif
            RestClient.Post(options).Then(response =>
                {
                    utcs.TrySetResult(DeserializeData<T>(response));
                })
                .Catch(err =>
                {
                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Post)}. {err.Message}. Url: {options.Uri}");
                    utcs.TrySetResult(DeserializeData<T>(null));
                });

            return await utcs.Task;
        }

        private async UniTask<HttpResponse<T>> Post<T>(RequestHelper options) where T : class
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<T>>();

            HttpResponse<T> httpResponse = null;
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Post(options).Then(response =>
                {
                    httpResponse = new HttpResponse<T>(DeserializeData<T>(response), response.StatusCode);
                    utcs.TrySetResult(httpResponse);
                })
                .Catch(err =>
                {
                    RequestException error = null;
                
                    if((err is RequestException))
                        error = (RequestException)err;

                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Post)}. {err.Message}. Url: {options.Uri}." +
                                     $"Error code: {error.StatusCode}" +
                                     $" Error message: {error.Response}");
                    httpResponse = new HttpResponse<T>(default,
                        (err as RequestException).StatusCode);
                    utcs.TrySetResult(httpResponse);
                });

            return await utcs.Task;
        }

        private async UniTask<HttpResponse<bool>> PostBool(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<bool>>();

            HttpResponse<bool> httpResponse = null;
            if(!options.Headers.ContainsKey("Accept-Encoding"))
                options.Headers.Add("Accept-Encoding","gzip");

            RestClient.Post(options).Then(response =>
                {
                    httpResponse = new HttpResponse<bool>(true, response.StatusCode);
                    utcs.TrySetResult(httpResponse);
                })
                .Catch(err =>
                {
                    Debug.LogWarning($"{typeof(APIService).Name}. {nameof(Post)}. {err.Message}. Url: {options.Uri}");
                    httpResponse = new HttpResponse<bool>(default,
                        (err as RequestException).StatusCode);
                    utcs.TrySetResult(httpResponse);
                });

            return await utcs.Task;
        }

        private async UniTask<HttpResponse<bool>> GetBool(RequestHelper options)
        {
            var utcs = new UniTaskCompletionSource<HttpResponse<bool>>();

            HttpResponse<bool> httpResponse = null;

            RestClient.Get(options).Then(response =>
                {
                    bool.TryParse(response.Text, out bool result);
                    httpResponse = new HttpResponse<bool>(result, response.StatusCode);
                    utcs.TrySetResult(httpResponse);
                })
                .Catch(err =>
                {
                    Debug.LogWarning(
                        $"{typeof(APIService).Name}. {nameof(GetBool)}. {err.Message}. Url: {options.Uri}");
                    httpResponse = new HttpResponse<bool>(default, (err as RequestException).StatusCode);
                    utcs.TrySetResult(httpResponse);
                });

            return await utcs.Task;
        }

        public void SetClientData(GetTokenResponse tokenResponse)
        {
            if (!ReferenceEquals(tokenResponse, null))
            {
                _clientData.TypeOwner = 0;
                _clientData.UserId = tokenResponse.UserId;
                _clientData.UserName = tokenResponse.Login;
                _clientData.OwnedSpacesNumber = tokenResponse.OwnedSpacesNumber;
                _clientData.IsGuest = tokenResponse.IsGuest;
                SetTokenInfo(tokenResponse.AccessToken, tokenResponse.RefeshToken);
                SetClientAvatar(tokenResponse.AvatarNumber);
                SetClientWatch(tokenResponse.AvatarWatchNumber, tokenResponse.OculusAvatarWatchNumber);

                AuthSuccess?.Invoke();
            }
        }

        private void SetTokenInfo(string accessToken, string refreshToken)
        {
            TokenDetails tokenDetails = GetTokenDetails(accessToken);

            _clientData.AccessToken = accessToken;
            _clientData.RefreshToken = refreshToken;

            _clientData.AuthTokenUnixTime = tokenDetails.auth_time.ToString();
            _clientData.ExpTokenUnixTime = tokenDetails.exp.ToString();
        }

        private void SetClientAvatar(int? avatarAssetId)
        {
            if (avatarAssetId.HasValue)
            {
                AvatarSessionData.SetAvatarAssetId(avatarAssetId.Value);
            }
            else
            {
                AvatarSessionData.SetAvatarAssetId(AvatarSessionData.DefaultAvatarAssetId);
            }
        }

        private void SetClientWatch(int? avatarWatchNumber, int? oculusAvatarWatchNumber)
        {
            if (oculusAvatarWatchNumber.HasValue) WatchSessionData.WatchPlayerIdOculus = oculusAvatarWatchNumber.Value;
            if (avatarWatchNumber.HasValue) WatchSessionData.WatchPlayerId = avatarWatchNumber.Value;
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

        private TokenDetails GetTokenDetails(string token)
        {
            var parts = token.Split('.');
            TokenDetails currentUserToken = null;
            if (parts.Length > 2)
            {
                var decode = parts[1];
                var padLength = 4 - decode.Length % 4;
                if (padLength < 4)
                {
                    decode += new string('=', padLength);
                }

                var bytes = System.Convert.FromBase64String(decode);
                var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);

                currentUserToken = JsonConvert.DeserializeObject<TokenDetails>(userInfo);
            }

            return currentUserToken;
        }

        public async UniTask<bool> SetWatch(bool isOculusAvatar, int? watchNumber)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            if (_clientData == null || !_clientData.UserId.HasValue)
            {
                Debug.LogWarning($"{typeof(APIService).Name}. {nameof(SetWatch)}. " +
                                 $"ClientData.UserId == null. Cannot set watch.");
                utcs.TrySetResult(false);
                return await utcs.Task;
            }

            string watchIdFormValue = watchNumber.HasValue ? watchNumber.Value.ToString() : null;

            WWWForm myWatchUpdateData = new();
            HttpResponse<bool> httpResponse;

            myWatchUpdateData.AddField("UserId", _clientData.UserId.ToString());
            if (isOculusAvatar)
            {
                myWatchUpdateData.AddField("OculusAvatarWatchNumber", watchIdFormValue);
                var options = new RequestHelper
                {
                    Uri = SetOculusAvatarWatchUrl,
                    FormData = myWatchUpdateData,
                };
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
                httpResponse = await PostBool(options);
            }
            else
            {
                myWatchUpdateData.AddField("AvatarWatchNumber", watchIdFormValue);
                var options = new RequestHelper
                {
                    Uri = SetAvatarWatchUrl,
                    FormData = myWatchUpdateData,
                };
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
                httpResponse = await PostBool(options);
            }

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (watchNumber.HasValue)
                {
                    if (isOculusAvatar) SetClientWatch(null, watchNumber.Value);
                    else SetClientWatch(watchNumber.Value, null);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(SetWatch)}. Cannot set avatar watch. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
            }

            utcs.TrySetResult(success);
            return await utcs.Task;
        }

        #endregion
    }
}