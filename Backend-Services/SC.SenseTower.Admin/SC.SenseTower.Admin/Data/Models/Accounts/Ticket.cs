using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Accounts
{
    /// <summary>
    /// Билеты.
    /// </summary>
    [CollectionName(DbConstants.COLLECTION_TICKETS)]
    [BsonIgnoreExtraElements]
    public class Ticket
    {
        [BsonId]
        public string Id { get; set; } = null!;

        public Guid? IssuerId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UsingInfo UsingInfo { get; set; } = new();

        public RecallInfo RecallInfo { get; set; } = new();
    }
}
