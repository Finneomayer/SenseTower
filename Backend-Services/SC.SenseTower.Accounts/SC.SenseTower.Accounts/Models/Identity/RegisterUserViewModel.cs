using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Accounts.Models.Identity
{
    public class RegisterUserViewModel
    {
        [Display(Name = "Код приглашения")]
        [Required]
        public string Code { get; set; } = null!;

        public string? Method { get; set; }

        public string? Url { get; set; }

        public string? ButtonText { get; set; }
    }
}
