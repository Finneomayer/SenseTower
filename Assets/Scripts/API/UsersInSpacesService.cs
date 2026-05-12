using System;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Scripts.API
{
    public class UsersInSpacesService : IUsersInSpacesService
    {
        private IApiService _apiService;
        private bool _isRequestInProgress;

        [Inject]
        public void Construct(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async UniTask<UsersInSpaceResponse> GetUsersInSpaces(int? count)
        {
            if (_isRequestInProgress)
            {
                await UniTask.WaitWhile(() => _isRequestInProgress);
            }

            _isRequestInProgress = true;

            var utcs = new UniTaskCompletionSource<UsersInSpaceResponse>();

            string url = APIService.UsersInSpacesUrl;
            if (count.HasValue)
            {
                url += $"?getCount={count.Value}";
            }
            url = APIService.AddLanguageParameter(url);

            UsersInSpaceResponse result = await _apiService.GetWithAuthToken<UsersInSpaceResponse>(url);
            
            utcs.TrySetResult(result);

            _isRequestInProgress = false;

            return await utcs.Task;
        }
    }
}