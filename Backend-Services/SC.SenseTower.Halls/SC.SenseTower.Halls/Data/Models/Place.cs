using MongoDB.Bson.Serialization.Attributes;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Halls.Data.Models
{
    [BsonIgnoreExtraElements]
    public class Place
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public string PlaceName { get; set; } = null!;

        public Guid? OwnerId { get; set; }

        public string? OwnerName { get; set; }
        
        public ImageInfo? DoorImage { get; set; }

        public Picture[]? Images { get; set; }

        public Space? LocalSpace { get; set; }

        public AccessType PublicAccessType { get; set; }
    }
}
