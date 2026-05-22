using MongoDB.Bson.Serialization.Attributes;

namespace SC.SenseTower.Halls.Data.Models
{
    [BsonIgnoreExtraElements]
    public class ImageInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string PreviewUrl { get; set; } = null!;
    }
}
