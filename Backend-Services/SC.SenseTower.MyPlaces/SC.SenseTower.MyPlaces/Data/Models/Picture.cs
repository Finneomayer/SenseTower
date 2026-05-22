using MongoDB.Bson.Serialization.Attributes;

namespace SC.SenseTower.MyPlaces.Data.Models
{
    [BsonIgnoreExtraElements]
    public class Picture
    {
        public int Location { get; set; }

        public Guid? ImageId { get; set; }

        public ImageInfo Image { get; set; } = new();
    }
}
