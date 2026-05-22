using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Accounts.Constants;

namespace SC.SenseTower.Accounts.Data.Models
{
    /// <summary>
    /// Инвайты.
    /// </summary>
    [CollectionName(DbConstants.COLLECTION_INVITES)]
    public class Invite
    {
        [BsonId]
        public string Id { get; set; } = null!;

        public Guid? IssuerId { get; set; }

        public Guid? AuthorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UsingInfo UsingInfo { get; set; } = new();

        public RecallInfo RecallInfo { get; set; } = new();
    }
}
