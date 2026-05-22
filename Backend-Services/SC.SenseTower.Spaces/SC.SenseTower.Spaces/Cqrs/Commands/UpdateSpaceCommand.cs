using MediatR;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Spaces.Cqrs.Commands
{
    public class UpdateSpaceCommand : IRequest<Unit>
    {
        /// <summary>
        /// Идентификатор пространства.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя пространства.
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
        /// Порт для соединения с сервером пространства.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// IP-адрес сервера пространства.
        /// </summary>
        public string Ip { get; set; } = null!;

        /// <summary>
        /// Номер пространства.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Идентификатор владельца пространства.
        /// </summary>
        public Guid? SpaceOwnerId { get; set; }

        /// <summary>
        /// Тип доступа в пространство.
        /// </summary>
        public AccessType PublicAccessType { get; set; } = AccessType.Public;

        /// <summary>
        /// Признак частного пространства.
        /// </summary>
        public bool IsPrivate { get; set; } = true;

        /// <summary>
        /// Идентификатор изображения на двери пространства.
        /// </summary>
        public Guid? DoorImageId { get; set; }

        /// <summary>
        /// Токен доступа текущего пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;
    }
}
