using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Dto.Spaces;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate
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

        public int Number { get; set; }

        public UserInfoDto? SpaceOwner { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public bool IsPrivate { get; set; } = true;

        public ImageInfo? DoorImage { get; set; }
    }
}
