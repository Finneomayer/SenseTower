using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDbGenericRepository;

namespace SC.SenseTower.Common.Services
{
    public class BaseDbService : BaseService
    {
        protected readonly MongoDbContext context;

        public BaseDbService(
            ILogger logger,
            IMapper mapper,
            MongoDbContext context) : base(logger, mapper)
        {
            this.context = context;
        }
    }
}
