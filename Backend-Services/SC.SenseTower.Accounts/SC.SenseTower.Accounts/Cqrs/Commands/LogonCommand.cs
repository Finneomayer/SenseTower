using MediatR;
using SC.SenseTower.Accounts.Dto.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SC.SenseTower.Accounts.Cqrs.Commands
{
    /// <summary>
    /// old
    /// </summary>
    public class LogonCommand : IRequest<LogonResultDto>
    {
        /// <summary>
        /// Имя входа.
        /// </summary>
        [Required]
        public string Login { get; set; } = null!;

        /// <summary>
        /// Пароль.
        /// </summary>
        [Required]
        public string Password { get; set; } = null!;
    }

    /// <summary>
    /// refactor candidate
    /// </summary>
    public class LoginCommand : IRequest<LogonResultDto>
    {
        /// <summary>
        /// Имя входа.
        /// </summary>
        [Required]
        [FromBody]
        public string Login { get; set; } = null!;

        /// <summary>
        /// Пароль.
        /// </summary>
        [Required]
        [FromBody]
        public string Password { get; set; } = null!;
    }
}
