using AutoMapper;
using Octokit;
using SC.SenseTower.Common.Services;
using System.Text;

namespace SC.SenseTower.Utilities.Services
{
    public class RepositoryService
    {
        /// <summary>
        /// Имя организации.
        /// </summary>
        private const string ORGANIZATION_NAME = "GitHubOrganization";
        private readonly string organizationName;

        /// <summary>
        /// Персональный токен доступа для сервисного аккаунта, от имени которого выполняются операции в GitHub.
        /// </summary>
        private const string CREDENTIALS_TOKEN = "GitHubPersonalAccessToken";
        private readonly string accessToken;

        private readonly GitHubClient client;
        private readonly Credentials credentials;

        public RepositoryService()
        {
            organizationName = Environment.GetEnvironmentVariable(ORGANIZATION_NAME) ?? "Sense-Capital";
            accessToken = Environment.GetEnvironmentVariable(CREDENTIALS_TOKEN) ?? "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";

            client = new GitHubClient(new ProductHeaderValue(organizationName));
            credentials = new Credentials(accessToken);
        }

        public async Task<string> GetFile(string file)
        {
            client.Credentials = credentials;
            var repository = (await client.Repository.GetAllForCurrent()).FirstOrDefault(r => r.Name == "Automatization");
            if (repository == null)
                throw new Exception("Репозиторий не найден");
            var content = await client.Repository.Content.GetAllContents(repository.Id, file);
            if (content == null || content.Count == 0)
                throw new Exception($"Файл {file} не найден");
            return content[0].Content;
        }
    }
}
