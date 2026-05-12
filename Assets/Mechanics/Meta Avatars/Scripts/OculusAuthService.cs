using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using Oculus.Avatar2;
using Cysharp.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    public class OculusAuthService : IOculusAuthService
    {
        private const double RequestTimeoutMs = 10000;
        
        private ulong? _loggedInUserId;
        private bool _isAuthInProgress;

        public async UniTask<bool> IsCdnAvailable()
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(RequestTimeoutMs);
            try
            {
                var response = await client.GetAsync("https://fbcdn.net");
                utcs.TrySetResult(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Users custom avatars are not available: {e}");
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> IsUserHasAvatar(ulong userId)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var userHasAvatarRequest = await OvrAvatarManager.Instance.UserHasAvatarAsync(userId);

            if (userHasAvatarRequest != OvrAvatarManager.HasAvatarRequestResultCode.HasAvatar)
            {
                Debug.LogWarning($"User has no avatar");
            }

            utcs.TrySetResult(userHasAvatarRequest == OvrAvatarManager.HasAvatarRequestResultCode.HasAvatar);
            return await utcs.Task;
        }

        public async UniTask<ulong> LogInUser()
        {
            var utcs = new UniTaskCompletionSource<ulong>();

            if (_isAuthInProgress)
            {
                await UniTask.WaitWhile(() => _isAuthInProgress);
            }

            if (_loggedInUserId.HasValue)
            {
                utcs.TrySetResult(_loggedInUserId.Value);
                return await utcs.Task;
            }

            _isAuthInProgress = true;
            _loggedInUserId = 0;

            bool platformInitialized = await InitPlatform();
            if (!platformInitialized)
            {
                _isAuthInProgress = false;
                utcs.TrySetResult(_loggedInUserId.Value);
                return await utcs.Task;
            }

            bool isUserEntitledToApplication = await IsUserEntitledToApplication();
            if (!isUserEntitledToApplication)
            {
                _isAuthInProgress = false;
                utcs.TrySetResult(_loggedInUserId.Value);
                return await utcs.Task;
            }

            string userAccessToken = await GetUserAccessToken();
            if (string.IsNullOrEmpty(userAccessToken))
            {
                _isAuthInProgress = false;
                utcs.TrySetResult(_loggedInUserId.Value);
                return await utcs.Task;
            }

            OvrAvatarEntitlement.SetAccessToken(userAccessToken);

            User loggedInUser = await GetLoggedInUser();
            if (loggedInUser == null)
            {
                _isAuthInProgress = false;
                utcs.TrySetResult(_loggedInUserId.Value);
                return await utcs.Task;
            }

            _isAuthInProgress = false;
            _loggedInUserId = loggedInUser.ID;
            utcs.TrySetResult(_loggedInUserId.Value);
            return await utcs.Task;
        }

        private async UniTask<bool> InitPlatform()
        {
            var utcs = new UniTaskCompletionSource<bool>();

            Message<PlatformInitialize> initCompleteMessage = null;
            Core.AsyncInitialize().OnComplete(message => { initCompleteMessage = message; });
            
            await UniTask.WaitUntil(() => initCompleteMessage != null);
            
            if (initCompleteMessage.IsError)
            {
                Debug.LogWarningFormat("Oculus: Error during initialization. Error Message: {0}",
                    initCompleteMessage.GetError().Message);
                utcs.TrySetResult(false);
            }
            else
            {
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        private async UniTask<bool> IsUserEntitledToApplication()
        {
            var utcs = new UniTaskCompletionSource<bool>();

            Message completeMessage = null;
            Entitlements.IsUserEntitledToApplication().OnComplete(message => { completeMessage = message; });

            await UniTask.WaitUntil(() => completeMessage != null);

            if (completeMessage.IsError)
            {
                Debug.LogWarningFormat("Oculus: Error verifying the user is entitled to the application. Error Message: {0}",
                    completeMessage.GetError().Message);
                utcs.TrySetResult(false);
            }
            else
            {
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        private async UniTask<string> GetUserAccessToken()
        {
            var utcs = new UniTaskCompletionSource<string>();

            Message<string> completeMessage = null;
            Users.GetAccessToken().OnComplete(message => { completeMessage = message; });

            await UniTask.WaitUntil(() => completeMessage != null);

            if (completeMessage.IsError)
            {
                Debug.LogWarningFormat("Oculus: Error getting access token. Error Message: {0}",
                    completeMessage.GetError().Message);
                utcs.TrySetResult(null);
            }
            else
            {
                utcs.TrySetResult(completeMessage.Data);
            }

            return await utcs.Task;
        }

        private async UniTask<User> GetLoggedInUser()
        {
            var utcs = new UniTaskCompletionSource<User>();

            Message<User> completeMessage = null;
            Users.GetLoggedInUser().OnComplete(message => { completeMessage = message; });

            await UniTask.WaitUntil(() => completeMessage != null);

            if (completeMessage.IsError)
            {
                Debug.LogWarningFormat("Oculus: Error getting logged in user. Error Message: {0}",
                    completeMessage.GetError().Message);
                utcs.TrySetResult(null);
            }
            else
            {
                utcs.TrySetResult(completeMessage.Data);
            }

            return await utcs.Task;
        }
    }

}
