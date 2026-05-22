using Microsoft.Extensions.Logging;

namespace SC.SenseTower.Common.Services.Yandex
{
    public class BaseStorageService
    {
        protected ILogger logger;

        public BaseStorageService(ILogger logger)
        {
            this.logger = logger;
        }
    }
}
