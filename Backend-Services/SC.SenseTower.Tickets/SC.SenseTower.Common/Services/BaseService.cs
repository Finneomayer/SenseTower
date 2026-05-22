using AutoMapper;
using Microsoft.Extensions.Logging;

namespace SC.SenseTower.Common.Services
{
    public class BaseService
    {
        protected readonly ILogger Logger;
        protected readonly IMapper Mapper;

        public BaseService(ILogger logger, IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
        }
    }
}
