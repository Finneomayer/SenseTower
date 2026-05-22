using SC.SenseTower.Common.Enums;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Dto.Accounts;

namespace SC.SenseTower.Spaces.Dto.Spaces
{
    public class SpaceItemDto
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

        /// <summary>
        /// Номер пространства.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Владелец пространства.
        /// </summary>
        public UserInfoDto? SpaceOwner { get; set; }

        /// <summary>
        /// Тип доступа в пространство.
        /// </summary>
        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        /// <summary>
        /// Признак частного пространства.
        /// </summary>
        public bool IsPrivate { get; set; } = true;

        /// <summary>
        /// Изображение на двери пространства.
        /// </summary>
        public ImageInfoDto? DoorImage { get; set; }

        /// <summary>
        /// Список изображений внутри пространства.
        /// </summary>
        public Dictionary<int, ImageInfoDto> Images { get; set; } = new();
    }
}
