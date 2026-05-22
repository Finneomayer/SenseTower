using MongoDB.Bson.Serialization.Attributes;

namespace SC.SenseTower.Cinemas.Data.Models
{
    [BsonIgnoreExtraElements]
    public class UserInfo
    {
        public Guid Id { get; set; }

        public string Role { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int? AvatarNumber { get; set; }
    }
}
