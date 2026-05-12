using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Assets.Scripts.Server;
using UnityEngine;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Models;
using Assets.Scripts.Client;
using System.Data;
using System.Linq;
using Assets.Scripts.API;

namespace Assets.Mechanics.Mafia
{
    public class MafiaGameService : IMafiaGameService
    {
        private IClientData _clientData;
        private IServerApiData _serverApiData;

        public void Init(IServerApiData serverApiData)
        {
            _serverApiData = serverApiData;
        }
        public void InitAsClient(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<MafiaGameStateDto> StartNewGame(string tableId, string adminId, string[] playerIds, MafiaPlayerRole[] userRolesPreset)
        {
            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            List<Guid> playerGuids = new List<Guid>();

            foreach (var playerId in playerIds)
            {
                playerGuids.Add(Guid.Parse(playerId));
            }

            List<MafiaPlayerRole> roles = userRolesPreset == null ? null : userRolesPreset.ToList();

            CreateMafiaGameSessionCommand sendData = new CreateMafiaGameSessionCommand()
            {
                TableId = tableId,
                IsAutoGenerateRoles = true,
                Players = playerGuids,
                GameMasterUserId = Guid.Parse(adminId),
                UserRolesPreset = roles
            };

            MafiaGameStateDto gameSession = await CreateGameSession(sendData);

            if (gameSession != null)
            {
                Debug.Log($"Game created. Status: {gameSession.GameStage}");
            }
            else
            {
                Debug.Log($"Game {tableId} was not created!. AdminId: {adminId}, Players: {playerIds}");
            }

            utcs.TrySetResult(gameSession);
            return await utcs.Task;
        }

        public async UniTask<bool> CompleteGame(string tableId)
        {
            Debug.LogWarning("*EndGameSession");
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetMafiaGameEndPoint));
            string url = $"{ServerApiService.GetMafiaGameEndPoint}/{tableId}";

            HttpResponse<string> httpResponse =
                await WebRequestFunctions.Delete(url, _serverApiData.AccessToken);

            if (httpResponse != null && httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                Debug.LogWarning($"{nameof(CompleteGame)}. Cannot delete game session. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

        public async UniTask<MafiaGameStateDto> GetCurrentState(string tableId)
        {
            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetMafiaGameEndPoint));
            string url = $"{ServerApiService.GetMafiaGameEndPoint}/{tableId}";

            HttpResponse<ScResult<MafiaGameStateDto>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<MafiaGameStateDto>>
                    (url, _serverApiData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"Table {tableId} game stage: {httpResponse.ResponseData.Result.GameStage}");
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetCurrentState)}. Cannot get current game state {tableId}. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        public async UniTask<MafiaGameStateDto> GoToNextStage(string tableId, NextMafiaGameStageRequestDto sendingData)
        {
            Debug.Log($"GoToNextStage");

            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            MafiaGameStateDto gameState = await UpdateGameSession(tableId, sendingData);

            utcs.TrySetResult(gameState);
            return await utcs.Task;
        }

        public async UniTask<MafiaGameStateDto> KickPlayer(string tableId, Guid playerId)
        {
            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetMafiaGameEndPoint));
            string url = $"{ServerApiService.GetMafiaGameEndPoint}/{tableId}/kick";

            KickMafiaPlayerRequestDto sendingData = new()
            {
                TableId = tableId,
                PlayerId = playerId
            };

            var httpResponse = await WebRequestFunctions.PostWithDeserialization
                <KickMafiaPlayerRequestDto, ScResult<MafiaGameStateDto>>(url, sendingData, _serverApiData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(KickPlayer)}. Cannot kick player. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        private async UniTask<MafiaGameStateDto> CreateGameSession(CreateMafiaGameSessionCommand sendData)
        {
            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            #region Debug players
            //switch (sendData.Players.Count)
            //{
            //    case 0:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        sendData.Players.Add(Guid.Parse("2e01f4d3-da96-472c-a7be-388cd06a1137"));
            //        sendData.Players.Add(Guid.Parse("a807e1f0-140e-40df-a469-da26fc902d27"));
            //        sendData.Players.Add(Guid.Parse("edc8c4c1-0b32-4677-be71-8ebe9efb23d7"));
            //        sendData.Players.Add(Guid.Parse("48607d4c-baa3-4f8e-852e-6c0845053048"));
            //        sendData.Players.Add(Guid.Parse("01f08ff2-91a0-4f55-a61f-f2ad420f435c"));
            //        break;
            //    case 1:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        sendData.Players.Add(Guid.Parse("2e01f4d3-da96-472c-a7be-388cd06a1137"));
            //        sendData.Players.Add(Guid.Parse("a807e1f0-140e-40df-a469-da26fc902d27"));
            //        sendData.Players.Add(Guid.Parse("edc8c4c1-0b32-4677-be71-8ebe9efb23d7"));
            //        sendData.Players.Add(Guid.Parse("48607d4c-baa3-4f8e-852e-6c0845053048"));
            //        break;
            //    case 2:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        sendData.Players.Add(Guid.Parse("2e01f4d3-da96-472c-a7be-388cd06a1137"));
            //        sendData.Players.Add(Guid.Parse("a807e1f0-140e-40df-a469-da26fc902d27"));
            //        sendData.Players.Add(Guid.Parse("edc8c4c1-0b32-4677-be71-8ebe9efb23d7"));
            //        break;
            //    case 3:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        sendData.Players.Add(Guid.Parse("2e01f4d3-da96-472c-a7be-388cd06a1137"));
            //        sendData.Players.Add(Guid.Parse("a807e1f0-140e-40df-a469-da26fc902d27"));
            //        break;
            //    case 4:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        sendData.Players.Add(Guid.Parse("2e01f4d3-da96-472c-a7be-388cd06a1137"));
            //        break;
            //    case 5:
            //        sendData.Players.Add(Guid.Parse("3ef08090-13aa-4333-b6d2-c9f55f6d13ae"));
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetMafiaGameEndPoint));
            string url = ServerApiService.GetMafiaGameEndPoint;
            var httpResponse = await WebRequestFunctions.PostWithDeserialization
                <CreateMafiaGameSessionCommand, ScResult<MafiaGameStateDto>>(url, sendData, _serverApiData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(CreateGameSession)}. Cannot create game session. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        private async UniTask<MafiaGameStateDto> UpdateGameSession(string tableId, NextMafiaGameStageRequestDto sendingData)
        {
            Debug.LogWarning("*UpdateGameSession");
            var utcs = new UniTaskCompletionSource<MafiaGameStateDto>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetMafiaGameEndPoint));
            string url = $"{ServerApiService.GetMafiaGameEndPoint}/{tableId}";

            var httpResponse = await WebRequestFunctions.PostWithDeserialization
                <NextMafiaGameStageRequestDto, ScResult<MafiaGameStateDto>>(url, sendingData, _serverApiData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(UpdateGameSession)}. Cannot update game session. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }


        /// <summary>
        /// Client method!
        /// </summary>
        /// <param name="playerCount"></param>
        /// <returns></returns>
        public async UniTask<MafiaPlayerRole[]> GetPlayerRolesPresetClient(int playerCount)
        {
            var utcs = new UniTaskCompletionSource<MafiaPlayerRole[]>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetMafiaEndPoint));
            string url = $"{APIService.GetMafiaEndPoint}/presets/{playerCount}";

            HttpResponse<ScResult<MafiaPlayerRole[]>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<MafiaPlayerRole[]>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"Preset for {playerCount} players: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetPlayerRolesPresetClient)}. Cannot get preset for {playerCount} players. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        public async UniTask<MafiaLocalizationResultDto> GetLocalizationContentClient()
        {
            var utcs = new UniTaskCompletionSource<MafiaLocalizationResultDto>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetMafiaEndPoint));
            string url = $"{APIService.GetMafiaEndPoint}/role-content";
            
            // TODO: Remove forcing ru language after translating to english
            //url = APIService.AddLanguageParameter(url);
            url = APIService.AddParameter(url, $"lang=ru");

            HttpResponse<ScResult<MafiaLocalizationResultDto>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<MafiaLocalizationResultDto>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"Mafia localization data: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetLocalizationContentClient)}. Cannot get Mafia localization data. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        public async UniTask<MafiaTicketCheckResultDto> CheckMafiaTicketClient(string tableId)
        {
            var utcs = new UniTaskCompletionSource<MafiaTicketCheckResultDto>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetMafiaEndPoint));
            string url = $"{APIService.GetMafiaEndPoint}/{tableId}/users/{_clientData.UserId}/access";

            url = APIService.AddLanguageParameter(url);

            HttpResponse<ScResult<MafiaTicketCheckResultDto>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<MafiaTicketCheckResultDto>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetLocalizationContentClient)}. Cannot check Mafia ticket. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }
    }
}
