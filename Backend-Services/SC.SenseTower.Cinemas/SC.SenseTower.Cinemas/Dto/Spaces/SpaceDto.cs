using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Cinemas.Dto.Spaces
{
    public class SpaceDto
    {
        /// <summary>
        /// Идентфикатор пространства.
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
        /// Имя сцены для Unity.
        /// </summary>
        public string SceneName { get; set; } = null!;

        /// <summary>
        /// Имя удаленной сцены для Unity.
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
        /// Информация о соединении с сервером.
        /// </summary>
        public SpaceConnectionInfoDto SpaceConnectionInfo { get; set; } = new();

        /// <summary>
        /// Номер пространства.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Владелец пространства.
        /// </summary>
        public Users.UserInfoDto? SpaceOwner { get; set; }

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
