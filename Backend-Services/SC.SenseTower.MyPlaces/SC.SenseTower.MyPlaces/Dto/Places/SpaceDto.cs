using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.MyPlaces.Dto.Places
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
        /// Информация о соединении с сервером пространства.
        /// </summary>
        public SpaceConnectionDto SpaceConnectionInfo { get; set; } = new();
    }
}
