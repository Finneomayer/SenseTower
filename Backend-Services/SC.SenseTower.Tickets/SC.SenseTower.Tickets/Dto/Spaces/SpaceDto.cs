using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Tickets.Dto.Spaces
{
    public class SpaceDto
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название пространства.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Тип пространства.
        /// </summary>
        public SpaceType SpaceType { get; set; }

        /// <summary>
        /// Имя сцены в Unity.
        /// </summary>
        public string SceneName { get; set; } = null!;

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
        /// Информация о соединении с сервером.
        /// </summary>
        public SpaceConnectionInfoDto SpaceConnectionInfo { get; set; } = new();
    }
}
