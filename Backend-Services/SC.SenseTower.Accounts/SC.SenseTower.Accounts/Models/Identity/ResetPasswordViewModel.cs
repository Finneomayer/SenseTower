using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Accounts.Models.Identity
{
    public class ResetPasswordViewModel
    {
        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;

        [Display(Name = "Новый пароль")]
        [Required]
        [StringLength(30, MinimumLength = 12)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Подтверждение пароля")]
        [Required]
        [StringLength(30, MinimumLength = 12)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
