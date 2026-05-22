using MediatR;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.Dto.Spaces;

namespace SC.SenseTower.Galleries.RabbitMQ.SpaceUpdate
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

        public ConnectionInfoDto SpaceConnectionInfo { get; set; } = new();

        public int Number { get; set; }

        public UserInfo? SpaceOwner { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public bool IsPrivate { get; set; } = true;

        public ImageInfo? DoorImage { get; set; }
    }
}
