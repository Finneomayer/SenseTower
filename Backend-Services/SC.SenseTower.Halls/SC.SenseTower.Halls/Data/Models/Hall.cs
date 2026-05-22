using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Halls.Constants;

namespace SC.SenseTower.Halls.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_HALLS)]
    [BsonIgnoreExtraElements]
    public class Hall
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public LocalSpace[] Spaces { get; set; } = Array.Empty<LocalSpace>();

        public Place[] UserPlaces { get; set; } = Array.Empty<Place>();

        public Space[] PublicPlaces { get; set; } = Array.Empty<Space>();

        public LocalSpace? Space { get; set; }
    }
}
