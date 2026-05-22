using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.MyPlaces.Constants;

namespace SC.SenseTower.MyPlaces.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_PLACES)]
    [BsonIgnoreExtraElements]
    public class Place
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("Name")]
        public string PlaceName { get; set; } = null!;

        public int PlaceNumber { get; set; }

        public Guid? OwnerId { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public Guid? DoorImageId { get; set; }

        public Picture[]? Images { get; set; }

        public Space? Space { get; set; }
    }
}
