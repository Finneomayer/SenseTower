using System;
using System.Collections.Generic;
using API.Models;
using API.Models.Registration;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Meta.WitAi.Json;
using Models;
using Proyecto26;
using UI;
using UnityEngine;
using static Data.Enumenators;

namespace Assets.Scripts.API.RegistrationService
{
    public class RegisterService : IRegistrationService
    {
        private IApiService _apiSerivce;
        private IClientData _clientData;

        private RegisterService(IClientData clientData, IApiService apiService)
        {
            _clientData = clientData;
            _apiSerivce = apiService;
        }

        public async UniTask<RegisterResult> Register(string login, string password, string email)
        {
            var utcs = new UniTaskCompletionSource<RegisterResult>();
            bool result = false;
            RegisterRequest reponse = new RegisterRequest() {Login = login, Password = password, Email = email};

            RequestHelper options = new RequestHelper();
            options.Uri = $"{APIService.RegistrationUrl}";
            options.BodyString = JsonConvert.SerializeObject(reponse);

            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<ScResult<bool>> registerResult =
                await WebRequestFunctions.PostWithDeserialization<ScResult<bool>>(options);

            result = registerResult.ResponseCode == HttpResponse<bool>.SuccessCode;
            utcs.TrySetResult(MapToRegisterResult(registerResult.ResponseData, result));
            return await utcs.Task;
        }

        public async UniTask<RegisterResult> MakeGuestResident(string login, string password, string email)
        {
            var utcs = new UniTaskCompletionSource<RegisterResult>();
            bool result = false;

            string userId = _clientData.UserId.ToString();

            RegisterRequest request = new RegisterRequest() { Login = login, Password = password, Email = email };
            RequestHelper options = new RequestHelper();
            var url = $"{APIService.MakeGuestResidentUrl.Replace($"{{0:userId}}", userId)}";
            
            options.Uri = url;
            options.BodyString = JsonConvert.SerializeObject(request);

            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<ScResult<bool>> registerResult =
                await WebRequestFunctions.PutWithDeserialization<ScResult<bool>>(options);

            result = registerResult.ResponseCode == HttpResponse<bool>.SuccessCode;
            utcs.TrySetResult(MapToRegisterResult(registerResult.ResponseData, result));
            return await utcs.Task;
        }

        public async UniTask<bool> RegisterAsGuest(string deviceId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            Guest guest = new();
            guest.DeviceId = deviceId;

            RequestHelper options = new RequestHelper();
            options.Uri = $"{APIService.RegistrationAsGuestUrl}";
            options.BodyString = JsonConvert.SerializeObject(guest);
           
            HttpResponse<GetTokenResponse> registerResult =
                await WebRequestFunctions.PostWithDeserialization<GetTokenResponse>(options);

            if (registerResult.ResponseCode == HttpResponse<GetTokenResponse>.SuccessCode)
            {
                _apiSerivce.SetClientData(registerResult.ResponseData);
            }

            utcs.TrySetResult(registerResult.ResponseCode == HttpResponse<GetTokenResponse>.SuccessCode);
            return await utcs.Task;
        }

        private RegisterResult MapToRegisterResult<T>(ScResult<T> message, bool success = false)
        {
            RegisterResult RegisterResult = new();
            RegisterResult.success = success;
            if (message.Error != null)
            {
                RegisterResult.ValidationError = new();
                RegisterResult.ValidationError.Message = message.Error.Message;

                foreach (KeyValuePair<string, List<string>> errors in message.Error.ModelState!)
                {
                    if (errors.Key.Equals(nameof(RegisterResult.ValidationError.Login))
                        || errors.Key.Equals(nameof(RegisterResult.ValidationError.InvalidUserName)))
                    {
                        RegisterResult.ValidationError.Login = string.Join(",", errors.Value);
                    }

                    if (errors.Key.Equals(nameof(RegisterResult.ValidationError.Password)))
                    {
                        RegisterResult.ValidationError.Password = string.Join(",", errors.Value);
                    }

                    if (errors.Key.Equals(nameof(RegisterResult.ValidationError.Email)))
                    {
                        RegisterResult.ValidationError.Email = string.Join(",", errors.Value);
                    }
                }
            }

            return RegisterResult;
        }
    }
}