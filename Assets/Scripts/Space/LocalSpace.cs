using System;
using System.Collections.Generic;
using Assets.Scripts.Space.SpaceSettings;

namespace Assets.Scripts.Space
{
    public sealed class LocalSpace
    {
        public SpaceConnectionInfo SpaceConnectionInfo { get; set; }
        public Guid Id { get; set; }
        public SpaceType SpaceType { get; set; }
        public string RemoteSceneName { get; set; }
        public string RemoteFolderName { get; set; }
        public string RemoteCatalogName { get; set; }
        public string SpaceName { get; set; }
        public string SceneName { get; set; }

        //from MyPlace
        public Owner SpaceOwner { get; set; }
        public List<string> AdminIds { get; set; }
        public List<string> BlockList { get; set; }
        public Dictionary<int, MyImage> Images { get; set; }
        public MyImage DoorImage { get; set; }
        public SpaceAccessType PublicAccessType { get; set; }
        public PublicAccessModeSettings PublicAccessModeSettings { get; set; }
        public bool IsPrivate { get; set; }
        public int Number { get; set; } = default;

        #region LikeService
        public int? LikesNumber { get; set; }
        public int? DislikesNumber { get; set; }
        public bool? CanLike { get; set; }
        public bool? IsForSale { get; set; }
        public bool? Like { get; set; }
        #endregion
    }
}