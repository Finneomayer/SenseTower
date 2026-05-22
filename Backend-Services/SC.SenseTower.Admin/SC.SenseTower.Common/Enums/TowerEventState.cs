using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Common.Enums
{
    public enum TowerEventState : byte
    {
        [Display(Name = "Запланировано")]
        Planned,

        [Display(Name = "Отменено")]
        Cancelled,

        [Display(Name = "Перенесено")]
        Moved
    }
}
