using System;
using System.Collections.Generic;
using System.Linq;
using Yandex.Cloud.Functions;
using YcAutomation.GithubBackups.Services.EmailSender;

namespace YcAutomation.GithubBackups
{
    public class Handler : YcFunction<BackupParameters, BackupResult>
    {
        private readonly GithubManager github;
        private readonly ObjectStoreManager objectStore;
        private readonly List<string> errors = new List<string>();

        public Handler()
        {
            github = new GithubManager();
            objectStore = new ObjectStoreManager();
        }

        public BackupResult FunctionHandler(BackupParameters request, Context context)
        {
            Console.WriteLine("Старт бэкапа репозиториев SenseTower");
            var result = new BackupResult();

            try
            {
                var repositories = github.GetRepositoryList().GetAwaiter().GetResult();
                foreach (var repository in repositories.Where(x => x.Size > 0))
                {
                    try
                    {
                        if (!objectStore.ObjectExists(repository.Name, ".zip", repository.PushedAt).GetAwaiter().GetResult())
                        {
                            var zip = github.GetZip(repository).GetAwaiter().GetResult();
                            objectStore.Save(repository.Name, ".zip", repository.PushedAt, zip).GetAwaiter().GetResult();
                        }
                        else
                            Console.WriteLine("Репозиторий {0} не изменился со времени последнего сохранения", repository.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        errors.Add($"Ошибка бэкапа репозитория {repository.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                errors.Add($"Ошибка бэкапа исходников: {ex.Message}");
            }

            var mailer = new EmailSenderService();
            var email = Environment.GetEnvironmentVariable("NotificationEmail") ?? "la.rabota.free@yandex.ru";
            if (errors.Count > 0)
            {
                mailer.SendEmailNotification(email, "Ошибки бэкапа исходников", string.Join("<br/>", errors)).GetAwaiter().GetResult();
            }
            else
            {
                mailer.SendEmailNotification(email, "Бэкап исходников", $"Бэкап исходников успешно выполнен {DateTime.UtcNow:yyyy.MM.dd HH:mm:ss}").GetAwaiter().GetResult();
            }

            Console.WriteLine("Завершение бэкапа репозиториев SenseTower");
            return result;
        }
    }
}
