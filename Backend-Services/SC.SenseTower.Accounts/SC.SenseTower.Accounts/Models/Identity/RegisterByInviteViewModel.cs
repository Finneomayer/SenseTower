using SC.SenseTower.Accounts.Cqrs.Commands;
using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Accounts.Models.Identity
{
    public class RegisterByInviteViewModel
    {
        /// <summary>
        /// Имя входа.
        /// </summary>
        [Display(Name = "Имя входа")]
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Login { get; set; } = null!;

        /// <summary>
        /// Email.
        /// </summary>
        [Display(Name = "Email")]
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль.
        /// </summary>
        [Display(Name = "Пароль")]
        [Required]
        [StringLength(30, MinimumLength = 12)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Display(Name = "Подтверждение пароля")]
        [Required]
        [StringLength(30, MinimumLength = 12)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        /// <summary>
        /// Список кошельков.
        /// </summary>
        public IEnumerable<AddWalletCommand>? Wallets { get; set; }

        /// <summary>
        /// Код приглашения.
        /// </summary>
        [Display(Name = "Код приглашения")]
        [Required]
        public string InviteId { get; set; } = null!;

        /// <summary>
        /// Признак регистрации через браузер.
        /// </summary>
        public bool FromUI { get; set; } = false;
    }
}
