using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Accounts.Models.Identity
{
    public class ResetRequestViewModel
    {
        [Display(Name = "Логин или Email")]
        [Required(ErrorMessage = "Не указаны логин или email")]
        public string? LoginOrEmail { get; set; }
    }
}
