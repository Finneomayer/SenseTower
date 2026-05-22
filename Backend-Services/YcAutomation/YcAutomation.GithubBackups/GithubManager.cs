using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YcAutomation.GithubBackups
{
    internal class GithubManager
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

        public GithubManager()
        {
            organizationName = Environment.GetEnvironmentVariable(ORGANIZATION_NAME) ?? "Sense-Capital";
            accessToken = Environment.GetEnvironmentVariable(CREDENTIALS_TOKEN) ?? "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";

            client = new GitHubClient(new ProductHeaderValue(organizationName));
            credentials = new Credentials(accessToken);
        }

        internal async Task<IReadOnlyList<Repository>> GetRepositoryList()
        {
            Console.Write("Читаем список репозиториев...");
            client.Credentials = credentials;
            var result = await client.Repository.GetAllForCurrent();//GetAllForOrg(organizationName);
            Console.WriteLine($" выполнено, получено репозиториев {result?.Count ?? 0}.");
            return result;
        }

        internal async Task<byte[]> GetZip(Repository repository)
        {
            Console.Write($"Читаем архив репозитория {repository.Name} размером {repository.Size:### ### ### ##0}КБ...");
            client.Credentials = credentials;
            var result = await client.Repository.Content.GetArchive(repository.Id, ArchiveFormat.Zipball);
            Console.WriteLine(" выполнено.");
            return result;
        }
    }
}