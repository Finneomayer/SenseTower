using System.ComponentModel.DataAnnotations;

namespace SC.SenseTower.Common.Enums
{
    /// <summary>
    /// Уровень доступа в помещение.
    /// </summary>
    public enum AccessType : byte
    {
        /// <summary>
        /// Все могут заходить в помещение.
        /// </summary>
        [Display(Name = "Публичный")]
        Public,

        /// <summary>
        /// Все могут заходить, если собственник в помещении.
        /// </summary>
        [Display(Name = "Только с владельцем")]
        Live,

        /// <summary>
        /// Может заходить только собственник.
        /// </summary>
        [Display(Name = "Только владельцу")]
        Private
    }
}
