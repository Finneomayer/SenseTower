using MediatR;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class LookupUsersRequest : IRequest<LookupItemDto<Guid>[]?>
    {
        /// <summary>
        /// Массив идентификаторов пользователей.
        /// </summary>
        public Guid[]? UserIds { get; set; }

        /// <summary>
        /// Имя роли для фильтрации.
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// Токен текущего пользователя (заполняется сервером).
        /// </summary>
        public string? AccessToken { get; set; }
    }
}
