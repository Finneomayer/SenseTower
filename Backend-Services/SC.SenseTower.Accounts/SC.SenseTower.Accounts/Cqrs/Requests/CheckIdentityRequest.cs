using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;

namespace SC.SenseTower.Accounts.Cqrs.Requests
{
    public class CheckIdentityRequest : IRequest<CheckIdentityResultDto[]>
    {
        /// <summary>
        /// Токен текущего пользователя (заполняется сервером).
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Массив проверяемых токенов.
        /// </summary>
        public CheckingToken[] Tokens { get; set; } = Array.Empty<CheckingToken>();
    }

    /// <summary>
    /// Элемент массива для проверки токенов.
    /// </summary>
    public class CheckingToken
    {
        /// <summary>
        /// Идентификатор проверяемого пользователя.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Проверяемый токен.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}
