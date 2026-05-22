using SC.SenseTower.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Admin.Models.Tickets
{
    public class UserTicketsViewModel
    {
        public string Title { get; set; } = null!;

        [Display(Name = "Пользователь")]
        public Guid? UserId { get; set; }

        public LookupItemDto<Guid>[] AvailableUsers { get; set; } = Array.Empty<LookupItemDto<Guid>>();

        [Display(Name = "Количество")]
        public int Quantity { get; set; } = 10;
    }
}
