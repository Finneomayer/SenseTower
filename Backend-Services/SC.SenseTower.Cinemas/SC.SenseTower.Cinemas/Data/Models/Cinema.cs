using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Cinemas.Constants;

namespace SC.SenseTower.Cinemas.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_CINEMAS)]
    [BsonIgnoreExtraElements]
    public class Cinema
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Space Space { get; set; } = new();

        public UserInfo[] Administrators { get; set; } = Array.Empty<UserInfo>();
    }
}
