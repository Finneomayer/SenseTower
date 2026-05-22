using MediatR;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.MyPlaces.Dto.Spaces;

namespace SC.SenseTower.MyPlaces.RabbitMQ.SpaceUpdate
{
    public class SpaceUpdateCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public string SpaceName { get; set; } = null!;

        public SpaceType SpaceType { get; set; }

        public string SceneName { get; set; } = null!;

        public string? RemoteSceneName { get; set; }

        public string? RemoteFolderName { get; set; }

        public string? RemoteCatalogName { get; set; }

        public SpaceMode SpaceMode { get; set; }

        public SpaceConnectionInfoDto SpaceConnectionInfo { get; set; } = new();
    }
}
