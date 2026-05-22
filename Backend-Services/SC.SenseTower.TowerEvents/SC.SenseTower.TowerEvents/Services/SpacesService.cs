using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.TowerEvents.Dto.Spaces;
using SC.SenseTower.TowerEvents.Settings;
using System.Text.Json;

namespace SC.SenseTower.TowerEvents.Services
{
    public class SpacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public SpacesService(
            ILogger<SpacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<LocalSpaceDto[]> GetAll(string accessToken, CancellationToken cancellationToken)
        {
            //var result = await Get<SpacesResponseDto>(accessToken, settings.GetAllSpacesUrl, null, cancellationToken);
            var json = @"
{
    ""result"": [
        {
            ""id"": ""e81e71b9-e685-40c7-9d4e-49f9a328c1db"",
            ""spaceConnectionInfo"": {
                ""port"": 7790,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 2,
            ""sceneName"": ""TheHallScene"",
            ""spaceName"": ""Лобби""
        },
        {
            ""id"": ""24b84aa8-909e-4d61-b942-4c2a63ba9af9"",
            ""spaceConnectionInfo"": {
                ""port"": 7791,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 4,
            ""sceneName"": ""TheLectureHall"",
            ""spaceName"": ""Лекторий""
        },
        {
            ""id"": ""17585806-0be3-4bb2-8f0e-0dfff489843c"",
            ""spaceConnectionInfo"": {
                ""port"": 7792,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 5,
            ""sceneName"": ""Cinema"",
            ""spaceName"": ""Кинотеатр""
        },
        {
            ""id"": ""51aaab1a-4f93-452d-95b2-b9b55f981f4c"",
            ""spaceConnectionInfo"": {
                ""port"": 7793,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 6,
            ""sceneName"": ""Standup"",
            ""spaceName"": ""Стендап""
        },
        {
            ""id"": ""1ab3c0cd-3ead-4d57-8374-d9c207f18941"",
            ""spaceConnectionInfo"": {
                ""port"": 7794,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 7,
            ""sceneName"": ""Meeting_small"",
            ""spaceName"": ""Переговорная""
        },
        {
            ""id"": ""a3db6e1f-4da8-4feb-8ca2-691137d32af5"",
            ""spaceConnectionInfo"": {
                ""port"": 7795,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 8,
            ""sceneName"": ""showroom"",
            ""spaceName"": ""Магазин""
        },
        {
            ""id"": ""ba7c8cdb-4cae-4c60-8725-e494a6f997ed"",
            ""spaceConnectionInfo"": {
                ""port"": 7796,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 9,
            ""sceneName"": ""ArtGallery"",
            ""spaceName"": ""Галерея""
        },
        {
            ""id"": ""423a299c-7e8a-46b3-b672-70dcc40e32ea"",
            ""spaceConnectionInfo"": {
                ""port"": 7801,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #1""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b73ef"",
            ""spaceConnectionInfo"": {
                ""port"": 7802,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #2""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7303"",
            ""spaceConnectionInfo"": {
                ""port"": 7803,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #3""
        },
        {
            ""id"": ""791aaf13-6692-493c-97fe-0bd4f4388bcf"",
            ""spaceConnectionInfo"": {
                ""port"": 7804,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #4""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7305"",
            ""spaceConnectionInfo"": {
                ""port"": 7805,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #5""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7306"",
            ""spaceConnectionInfo"": {
                ""port"": 7806,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #6""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7307"",
            ""spaceConnectionInfo"": {
                ""port"": 7807,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #7""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7308"",
            ""spaceConnectionInfo"": {
                ""port"": 7808,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #8""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7309"",
            ""spaceConnectionInfo"": {
                ""port"": 7809,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #9""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7310"",
            ""spaceConnectionInfo"": {
                ""port"": 7810,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #10""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7311"",
            ""spaceConnectionInfo"": {
                ""port"": 7811,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #11""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7312"",
            ""spaceConnectionInfo"": {
                ""port"": 7812,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #12""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7313"",
            ""spaceConnectionInfo"": {
                ""port"": 7813,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #13""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7315"",
            ""spaceConnectionInfo"": {
                ""port"": 7815,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #15""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7317"",
            ""spaceConnectionInfo"": {
                ""port"": 7817,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #17""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7320"",
            ""spaceConnectionInfo"": {
                ""port"": 7820,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #20""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7321"",
            ""spaceConnectionInfo"": {
                ""port"": 7821,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #21""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7322"",
            ""spaceConnectionInfo"": {
                ""port"": 7822,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #22""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7323"",
            ""spaceConnectionInfo"": {
                ""port"": 7823,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #23""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7324"",
            ""spaceConnectionInfo"": {
                ""port"": 7824,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #24""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7325"",
            ""spaceConnectionInfo"": {
                ""port"": 7825,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #25""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7326"",
            ""spaceConnectionInfo"": {
                ""port"": 7826,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #26""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7327"",
            ""spaceConnectionInfo"": {
                ""port"": 7827,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #27""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7328"",
            ""spaceConnectionInfo"": {
                ""port"": 7828,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #28""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7329"",
            ""spaceConnectionInfo"": {
                ""port"": 7829,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #29""
        },
        {
            ""id"": ""466c4f30-6b7d-4155-878e-08f80e9b7330"",
            ""spaceConnectionInfo"": {
                ""port"": 7830,
                ""ip"": ""51.250.89.118""
            },
            ""spaceType"": 3,
            ""sceneName"": ""TheOfficeScene"",
            ""spaceName"": ""Пространство #30""
        }
    ],
    ""id"": 46,
    ""exception"": null,
    ""status"": 5,
    ""isCanceled"": false,
    ""isCompleted"": true,
    ""isCompletedSuccessfully"": true,
    ""creationOptions"": 0,
    ""asyncState"": null,
    ""isFaulted"": false
}";
            var result = JsonSerializer.Deserialize<SpacesResponseDto>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return result?.Result ?? Array.Empty<LocalSpaceDto>();
        }

        public async Task<LocalSpaceDto?> Get(string accessToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<LocalSpaceDto>(accessToken, string.Format(endpointsSettings.GetSpaceUrl, id), null, cancellationToken);
            return result;
        }
    }
}
