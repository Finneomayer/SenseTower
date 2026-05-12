using System;
using System.Collections.Generic;
using API.Models;
using Assets.Scripts.Hall;
using Assets.Scripts.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.API
{
    public interface IApiService
    {
        public event Action ServerInitializedSuccess;
        public event Action FailServerInitialize;
        public event Action AuthSuccess;
        
        public UniTask Initialize(Assembly assembly);
        
        public UniTask<bool> Auth(WWWForm data);
        public UniTask<bool> AuthAsGuest(WWWForm data);
        public UniTask<bool> GetHall(WWWForm data);

        public void SetClientData(GetTokenResponse getTokenResponse);
        
        public UniTask<bool> SetAvatar(Guid UserId, int? avatarNumber);
        public UniTask<bool> SetWatch(bool isOculusAvatar, int? watchNumber);

        UniTask<bool> RefreshToken();
        UniTask<bool> UpdatePlaceAccessType(Guid myPlaceId, SpaceAccessType accessType, decimal tax = default);

        UniTask<T> GetWithAuthToken<T>(string Url) where T:class;
        UniTask<bool> PostWithAuthToken(string Url);
        UniTask<T> PostWithAuthToken<T>(string Url) where T:class;
        public bool CheckUserAuthentication();
        UniTask RefreshUserInfo();
    }
}