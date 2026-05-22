using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Admin.Models.Account
{
    public class LogonViewModel
    {
        [Display(Name = "Имя входа")]
        [Required(ErrorMessage = "Не указано имя входа.")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль.")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
