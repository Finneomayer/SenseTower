using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Admin.Constants;

namespace SC.SenseTower.Admin.Data.Models.Accounts
{
    /// <summary>
    /// Кошелёк пользователя.
    /// </summary>
    [CollectionName(DbConstants.COLLECTION_WALLETS)]
    [BsonIgnoreExtraElements]
    public class Wallet
    {
        /// <summary>
        /// Идентификатор кошелька пользователя.
        /// </summary>
        [BsonId]
        public string Id { get; set; } = null!;

        /// <summary>
        /// Название кошелька пользователя (опционально).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Идентификатор пользователя-владельца.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Кошелёк подтверждён.
        /// </summary>
        public bool IsConfirmed { get; set; } = false;

        /// <summary>
        /// Кошелёк активен.
        /// </summary>
        public bool IsActive { get; set; } = false;
    }
}
