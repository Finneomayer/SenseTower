using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Images.Constants;

namespace SC.SenseTower.Images.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_IMAGE_FILES)]
    [BsonIgnoreExtraElements]
    public class ImageFile
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        [BsonRequired]
        public string Name { get; set; } = null!;

        [BsonRequired]
        public string FileName { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string PreviewUrl { get; set; } = null!;
    }
}
