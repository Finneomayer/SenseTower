using MongoDB.Driver;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Common.Data
{
    public abstract class AbstractFilter<T>
    {
        public abstract Task<FilterDefinition<T>> Filter(BaseDbService? service, CancellationToken cancellationToken);
    }
}
