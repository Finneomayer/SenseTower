using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Spaces.Constants;
using SC.SenseTower.Spaces.Features.Likes;

namespace SC.SenseTower.Spaces.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_SPACES)]
    [BsonIgnoreExtraElements]
    public class Space
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("Name")]
        public string SpaceName { get; set; } = null!;

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

        public Picture[] Images { get; set; } = Array.Empty<Picture>();

        #region LikeService
        public Dictionary<string, bool> Likes { get; set; } = new();
        public int? LikesNumber => Likes.Values.Count(x => x);
        public int? DislikesNumber => Likes.Values.Count(x => !x);
        public bool? CanLike { get; set; } = false;

        #endregion
    }
}
