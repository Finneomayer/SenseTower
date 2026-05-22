using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Accounts
{
    /// <summary>
    /// Учетная запись пользователя.
    /// </summary>
    [CollectionName(DbConstants.COLLECTION_ACCOUNTS)]
    [BsonIgnoreExtraElements]
    public class Account
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid? ReferrerId { get; set; }

        public PasswordResetInfo? PasswordResetInfo { get; set; }
    }

    public class PasswordResetInfo
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiredAt { get; set; }
    }
}
