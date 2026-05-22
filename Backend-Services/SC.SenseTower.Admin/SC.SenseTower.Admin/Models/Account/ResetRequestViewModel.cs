using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Admin.Models.Account
{
    public class ResetRequestViewModel
    {
        [Display(Name = "Имя входа или email")]
        [Required(ErrorMessage = "Не указаны логин или email")]
        public string? LoginOrEmail { get; set; }
    }
}
