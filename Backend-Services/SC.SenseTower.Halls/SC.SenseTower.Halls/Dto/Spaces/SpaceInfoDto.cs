using SC.SenseTower.Common.Enums;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Dto.Spaces
{
    public class SpaceInfoDto
    {
        public Guid Id { get; set; }

        public string SpaceName { get; set; } = null!;

        public SpaceType SpaceType { get; set; }

        public string SceneName { get; set; } = null!;

        public string? RemoteSceneName { get; set; }

        public string? RemoteFolderName { get; set; }

        public string? RemoteCatalogName { get; set; }

        public SpaceMode SpaceMode { get; set; }

        public SpaceConnectionInfo SpaceConnectionInfo { get; set; } = new();

        public int Number { get; set; }

        public UserInfoDto? SpaceOwner { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public bool IsPrivate { get; set; } = true;

        public ImageDto? DoorImage { get; set; }

        public Dictionary<int, ImageDto> Images { get; set; } = new();
    }
}
