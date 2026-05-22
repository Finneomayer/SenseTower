using SC.SenseTower.Common.Enums;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Dto.Places
{
    public class LocalSpaceDto
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название пространства.
        /// </summary>
        public string SpaceName { get; set; } = null!;

        /// <summary>
        /// Тип пространства.
        /// </summary>
        public SpaceType SpaceType { get; set; }

        /// <summary>
        /// Имя сцены Unity.
        /// </summary>
        public string SceneName { get; set; } = null!;

        /// <summary>
        /// Имя удалённой сцены Unity.
        /// </summary>
        public string? RemoteSceneName { get; set; }

        /// <summary>
        /// Имя удалённой папки.
        /// </summary>
        public string? RemoteFolderName { get; set; }

        /// <summary>
        /// Имя удалённого каталога.
        /// </summary>
        public string? RemoteCatalogName { get; set; }

        /// <summary>
        /// Режим работы пространства.
        /// </summary>
        public SpaceMode SpaceMode { get; set; }

        /// <summary>
        /// Информация о соединении с сервером.
        /// </summary>
        public SpaceConnectionInfo SpaceConnectionInfo { get; set; } = new();

        public int Number { get; set; }

        public UserInfoDto? SpaceOwner { get; set; }

        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        public bool IsPrivate { get; set; } = true;

        public ImageDto? DoorImage { get; set; }

        public Dictionary<int, ImageDto> Images { get; set; } = new();
    }
}
