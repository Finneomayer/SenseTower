using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate
{
    public class SpaceInfoDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public SpaceType SpaceType { get; set; }

        public string SceneName { get; set; } = null!;

        public string? RemoteSceneName { get; set; }

        public string? RemoteFolderName { get; set; }

        public string? RemoteCatalogName { get; set; }

        public SpaceMode SpaceMode { get; set; }

        public SpaceConnectionInfoDto SpaceConnectionInfo { get; set; } = new();
    }
}
