using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services.Yandex;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Services
{
    public class ImageStorageService : BaseStorageService
    {
        private readonly StorageSettings storageSettings;
        private readonly AmazonS3Client cloud;

        public ImageStorageService(
            ILogger<ImageStorageService> logger,
            IOptions<StorageSettings> options) : base(logger)
        {
            storageSettings = options.Value;
            var configsS3 = new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net",
                LogResponse = true,
                AuthenticationRegion = storageSettings.Region,

            };
            cloud = new AmazonS3Client(storageSettings.KeyId, storageSettings.SecretKey, configsS3);
        }

        public string ComposeUrl(string fileName)
        {
            return fileName;
        }

        public async Task Save(Stream file, string name, CancellationToken cancellationToken)
        {
            using var transfer = new TransferUtility(cloud);
            await transfer.UploadAsync(file, storageSettings.BacketName, name, cancellationToken);
        }

        public async Task Delete(string name, CancellationToken cancellationToken)
        {
            await cloud.DeleteObjectAsync(storageSettings.BacketName, name, cancellationToken);
        }

        public async Task<Stream> Get(string name, CancellationToken cancellationToken)
        {
            using var transferUtility = new TransferUtility(cloud);
            var tempFile = Path.GetTempFileName();
            await transferUtility.DownloadAsync(tempFile, storageSettings.BacketName, name, cancellationToken);
            return File.OpenRead(tempFile);
        }
    }
}
