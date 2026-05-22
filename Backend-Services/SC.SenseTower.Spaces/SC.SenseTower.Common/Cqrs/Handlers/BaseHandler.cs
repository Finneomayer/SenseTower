using AutoMapper;
using Microsoft.Extensions.Logging;

namespace SC.SenseTower.Common.Cqrs.Handlers
{
    public class BaseHandler
    {
        protected readonly ILogger Logger;
        protected readonly IMapper Mapper;

        public BaseHandler(ILogger logger, IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
        }
    }
}
