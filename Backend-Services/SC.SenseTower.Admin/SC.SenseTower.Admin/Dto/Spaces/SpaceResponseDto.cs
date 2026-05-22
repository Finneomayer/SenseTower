using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Admin.Dto.Spaces
{
    public class SpaceResponseDto
    {
        public Guid Id { get; set; }

        public string SpaceName { get; set; } = null!;

        public SpaceType SpaceType { get; set; }

        public string SceneName { get; set; } = null!;

        public string? RemoteSceneName { get; set; }

        public string? RemoteFolderName { get; set; }

        public string? RemoteCatalogName { get; set; }

        public SpaceMode SpaceMode { get; set; }

        public ConnectionInfoResponseDto SpaceConnectionInfo { get; set; } = new();
    }
}
