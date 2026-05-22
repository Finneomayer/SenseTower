using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Admin.Models.Profile
{
    public class PasswordViewModel
    {
        [Display(Name = "Текущий пароль")]
        [Required(ErrorMessage = "Необходимо указать текущий пароль.")]
        public string? CurrentPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [Required(ErrorMessage = "Необходимо задать новый пароль.`")]
        public string? Password { get; set; }

        [Display(Name = "Подтверждение пароля")]
        [Required(ErrorMessage = "Необходимо повторно ввести новый пароль.")]
        [Compare("Password", ErrorMessage = "Подтверждение не соответствует новому паролю.")]
        public string? ConfirmPassword { get; set; }
    }
}
