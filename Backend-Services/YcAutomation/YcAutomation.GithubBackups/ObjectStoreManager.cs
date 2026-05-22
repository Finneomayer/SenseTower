using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YcAutomation.GithubBackups
{
    internal class ObjectStoreManager
    {
        /// <summary>
        /// Идентификатор ключа доступа пользователя для ObjectStore.
        /// </summary>
        private const string KEY_ID = "ObjectStoreKeyId";
        private readonly string keyId;

        /// <summary>
        /// Секрет ключа доступа пользователя для ObjectStore.
        /// </summary>
        private const string SECRET_KEY = "ObjectStoreSecretKey";
        private readonly string secretKey;

        /// <summary>
        /// Регион.
        /// </summary>
        private const string REGION = "ObjectStoreRegion";
        private readonly string region;

        /// <summary>
        /// Имя бакета в ObjectStore.
        /// </summary>
        private const string BACKET_NAME = "ObjectStoreBacketName";
        private readonly string backetName;

        private readonly AmazonS3Client cloud;

        public ObjectStoreManager()
        {
            keyId = Environment.GetEnvironmentVariable(KEY_ID) ?? "YCAJEsOmAX1puTu5m0KLp_SeY";
            secretKey = Environment.GetEnvironmentVariable(SECRET_KEY) ?? "YCMxOUrTBOr3BiRfJBFHIW-iM2684lqwvOyQBNJQ";
            region = Environment.GetEnvironmentVariable(REGION) ?? "ru-central1";
            backetName = Environment.GetEnvironmentVariable(BACKET_NAME) ?? "st-project-backups";

            var configsS3 = new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net",
                LogResponse = true,
                AuthenticationRegion = region,
                
            };
            cloud = new AmazonS3Client(keyId, secretKey, configsS3);
        }

        internal async Task<bool> ObjectExists(string name, string extension, DateTimeOffset? lastDate)
        {
            name += $"{(lastDate.HasValue ? $"-{lastDate.Value:yyyy-MM-dd-HH-mm-ss}" : string.Empty)}{extension}";
            var files = await cloud.ListObjectsAsync(backetName, name);
            return files.S3Objects.Any();
        }

        internal async Task Save(string name, string extension, DateTimeOffset? lastDate, byte[] content)
        {
            name += $"{(lastDate.HasValue ? $"-{lastDate.Value:yyyy-MM-dd-HH-mm-ss}" : string.Empty)}{extension}";
            Console.WriteLine("{0}: {1} byte(s)", name, content.Length);

            using var transfer = new TransferUtility(cloud);
            using var stream = new MemoryStream(content);
            await transfer.UploadAsync(stream, backetName, name);

            Console.WriteLine("{0} успешно загружен", name);
        }
    }
}