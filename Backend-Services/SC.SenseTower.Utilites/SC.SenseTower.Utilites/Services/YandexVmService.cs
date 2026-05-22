using AutoMapper;
using Jose;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Utilities.Dto.YandexVm;
using SC.SenseTower.Utilities.Settings;
using System.Security.Cryptography;
using System.Text.Json;

namespace SC.SenseTower.Utilities.Services
{
    public class YandexVmService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings settings;

        public YandexVmService(
            ILogger<YandexVmService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
        }

        public async Task UpdateDockerCompose(string file, string content, CancellationToken cancellationToken)
        {
            var token = await GetIamToken(cancellationToken);
            var instanceId = file.EndsWith("demo.yaml") ? settings.DemoInstanceId : settings.DevInstanceId;
            //var instance = await GetInstance(token, instanceId, cancellationToken);
            //if (instance == null)
            //    throw new Exception("Ошибка получения инстанса");

            //var url = $"instances/{instanceId}";
            //var payload = mapper.Map<UpdateInstanceDto>(instance);
            //payload.UpdateMask = "metadata";
            //payload.ServiceAccountId = settings.ServiceAccountId;
            //payload.Metadata["docker-compose"] = content;
            //var result = await PostAsJson<OperationResultDto>(token, url, payload, cancellationToken);

            var url = $"instances/{instanceId}/updateMetadata";
            var payload = new UpdateMetadataRequestDto();
            payload.Upsert.Add("docker-compose", content);
            var result = await PostAsJson<OperationResultDto>(token, url, payload, cancellationToken);
            if (result == null || result.Error != null)
            {
                throw new Exception("Ошибка обновления файла.");
            }
        }

        private async Task<InstanceDto?> GetInstance(string? token, string? instanceId, CancellationToken cancellationToken)
        {
            var result = await Get<InstanceDto>(token, $"instances/{instanceId}", new { view = "FULL" }, cancellationToken);
            return result;
        }

        private async Task<string?> GetIamToken(CancellationToken cancellationToken)
        {
            var jwt = GenerateJwtToken();
            var result = await PostAsJson<IamTokenResponseDto>(null, "https://iam.api.cloud.yandex.net/iam/v1/tokens", new { jwt = jwt }, cancellationToken);
            return result?.IamToken;
        }

        private string GenerateJwtToken()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var headers = new Dictionary<string, object>
            {
                { "kid", settings.OpenKeyId }
            };
            var payload = new Dictionary<string, object>()
            {
                { "aud", "https://iam.api.cloud.yandex.net/iam/v1/tokens" },
                { "iss", settings.ServiceAccountId },
                { "iat", now },
                { "exp", now + 3600 }
            };
            using var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText("./Keys/service-account-private.key").ToCharArray());
            var result = JWT.Encode(payload, rsa, JwsAlgorithm.PS256, headers);
            return result;
        }
    }
}
