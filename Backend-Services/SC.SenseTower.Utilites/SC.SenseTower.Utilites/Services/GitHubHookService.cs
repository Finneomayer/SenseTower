using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace SC.SenseTower.Utilities.Services
{
    public class GitHubHookService : WebhookEventProcessor
    {
        private readonly ILogger<GitHubHookService> logger;
        private readonly RepositoryService repositoryService;
        private readonly YandexVmService yandexVmService;

        public GitHubHookService(
            RepositoryService repositoryService,
            YandexVmService yandexVmService,
            ILogger<GitHubHookService> logger)
        {
            this.repositoryService = repositoryService;
            this.yandexVmService = yandexVmService;
            this.logger = logger;
        }
        protected override async Task ProcessPushWebhookAsync(WebhookHeaders headers, PushEvent pushEvent)
        {
            if (!(pushEvent.HeadCommit != null && pushEvent.HeadCommit.Modified.Any()))
            {
                return;
            }

            foreach (var file in pushEvent.HeadCommit.Modified.Where(x => x.EndsWith("compose-dev.yaml") || x.EndsWith("compose-demo.yaml")))
            {
                try
                {
                    var content = await repositoryService.GetFile(file);
                    await yandexVmService.UpdateDockerCompose(file, content, default);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Файл {file} не обновлён: {ex.Message}");
                }
            }
        }
    }
}
