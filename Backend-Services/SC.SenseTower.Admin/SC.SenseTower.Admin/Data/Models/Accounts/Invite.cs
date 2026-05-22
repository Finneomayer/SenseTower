using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Accounts
{
    /// <summary>
    /// Инвайты.
    /// </summary>
    [CollectionName(DbConstants.COLLECTION_INVITES)]
    [BsonIgnoreExtraElements]
    public class Invite
    {
        [BsonId]
        public string Id { get; set; } = null!;

        public Guid? IssuerId { get; set; }

        public Guid? AuthorId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public UsingInfo UsingInfo { get; set; } = new();

        public RecallInfo RecallInfo { get; set; } = new();
    }

    public class UsingInfo
    {
        public DateTime? Date { get; set; }

        public Guid? UserId { get; set; }
    }

    public class RecallInfo
    {
        public bool IsRecalled { get; set; }

        public DateTime? Date { get; set; }

        public string? RecallReason { get; set; }
    }
}
