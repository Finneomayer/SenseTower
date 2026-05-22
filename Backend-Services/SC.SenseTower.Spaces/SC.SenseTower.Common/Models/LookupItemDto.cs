namespace SC.SenseTower.Common.Models
{
    public class LookupItemDto<TKey>
    {
        /// <summary>
        /// Идентификатор элемента.
        /// </summary>
        public TKey Id { get; set; } = default!;

        /// <summary>
        /// Название элемента.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
