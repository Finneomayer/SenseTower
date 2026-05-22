using MongoDbGenericRepository.Attributes;
using SC.SenseTower.MyPlaces.Constants;

namespace SC.SenseTower.MyPlaces.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_COUNTERS)]
    public class Counter
    {
        public string Id { get; set; } = null!;

        public int Value { get; set; }
    }
}
