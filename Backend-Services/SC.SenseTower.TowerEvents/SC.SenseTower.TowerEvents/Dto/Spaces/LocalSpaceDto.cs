using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.TowerEvents.Dto.Spaces
{
    public class LocalSpaceDto
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Информация о соединении с сервером.
        /// </summary>
        public SpaceConnectionInfoDto? SpaceConnectionInfo { get; set; }

        /// <summary>
        /// Тип пространства.
        /// </summary>
        public SpaceType SpaceType { get; set; }

        /// <summary>
        /// Имя сцены в Unity.
        /// </summary>
        public string SceneName { get; set; } = null!;

        /// <summary>
        /// Название пространства.
        /// </summary>
        public string SpaceName { get; set; } = null!;

        /// <summary>
        /// Имя удаленной сцены в Unity.
        /// </summary>
        public string? RemoteSceneName { get; set; }

        /// <summary>
        /// Имя удаленной папки.
        /// </summary>
        public string? RemoteFolderName { get; set; }

        /// <summary>
        /// Имя удаленного каталога.
        /// </summary>
        public string? RemoteCatalogName { get; set; }

        /// <summary>
        /// Режим работы пространства.
        /// </summary>
        public SpaceMode SpaceMode { get; set; }

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
    }
}
