using System;
using System.Collections.Generic;
using Assets.Scripts.API;
using Cysharp.Threading.Tasks;
using UnityEditor;
using Zenject;

namespace Assets.Scripts.Gallery
{
    public class GalleryService : IGalleryService
    {
        private IApiService _apiservice;

        [Inject]
        private void Construct(IApiService apiService)
        {
            _apiservice = apiService;
        }

        public async UniTask<Gallery> GetById(string id)
        {
            string url = APIService.AddLanguageParameter($"{APIService.GetGalleryUrl}/{id}");
            return await _apiservice.GetWithAuthToken<Gallery>(url);
        }
    }
}