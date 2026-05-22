using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Data;
using SC.SenseTower.MyPlaces.Data.Models;

namespace SC.SenseTower.MyPlaces.Services
{
    public class CountersService : BaseDbService
    {
        private new readonly MyPlacesDbContext context;

        public CountersService(
            ILogger<CountersService> logger,
            IMapper mapper,
            MyPlacesDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as MyPlacesDbContext ?? null!;
        }

        public async Task<int> NextValue(string key, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Counter>.Filter.Eq(x => x.Id, key);
            var update = Builders<Counter>.Update.Inc(x => x.Value, 1);
            var counter = await context.Counters.FindOneAndUpdateAsync(
                filter,
                update,
                new FindOneAndUpdateOptions<Counter, Counter>
                {
                    BypassDocumentValidation = true
                },
                cancellationToken);
            if (counter == null)
            {
                counter = new Counter { Id = key, Value = 1 };
                await context.Counters.InsertOneAsync(
                    new Counter { Id = key, Value = 2 },
                    new InsertOneOptions { BypassDocumentValidation = true },
                    cancellationToken);
            }
            return counter.Value;
        }
    }
}
