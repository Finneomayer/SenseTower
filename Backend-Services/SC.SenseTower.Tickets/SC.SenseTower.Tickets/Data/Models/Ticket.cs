using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Tickets.Constants;

namespace SC.SenseTower.Tickets.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_TICKETS)]
    [BsonIgnoreExtraElements]
    public class Ticket
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public TowerEvent TowerEvent { get; set; } = new();
    }
}
