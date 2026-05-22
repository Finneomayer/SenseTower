using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Auth.Models
{
    public class RegisterModel
    {
        [Required]
        public string Login { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime? AccessGrantedTo { get; set; }
    }
}
