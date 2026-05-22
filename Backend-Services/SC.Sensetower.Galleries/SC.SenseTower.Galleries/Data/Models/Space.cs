using MongoDB.Bson.Serialization.Attributes;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Galleries.Data.Models
{
    [BsonIgnoreExtraElements]
    public class Space
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public SpaceType SpaceType { get; set; }

        public string SceneName { get; set; } = null!;

        public string? RemoteSceneName { get; set; }

        public string? RemoteFolderName { get; set; }

        public string? RemoteCatalogName { get; set; }

        public SpaceMode SpaceMode { get; set; }

        public SpaceConnectionInfo SpaceConnectionInfo { get; set; } = new();

        public int Number { get; set; }

        public UserInfo? SpaceOwner { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public bool IsPrivate { get; set; } = true;

        public ImageInfo? DoorImage { get; set; }
    }
}
