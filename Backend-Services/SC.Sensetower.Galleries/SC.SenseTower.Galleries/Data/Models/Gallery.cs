using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Galleries.Constants;

namespace SC.SenseTower.Galleries.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_GALLERIES)]
    [BsonIgnoreExtraElements]
    public class Gallery
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public GalleryInfoTable GalleryInfoTable { get; set; } = new();

        public Space Space { get; set; } = new();

        public Picture[] Pictures { get; set; } = Array.Empty<Picture>();
    }
}
