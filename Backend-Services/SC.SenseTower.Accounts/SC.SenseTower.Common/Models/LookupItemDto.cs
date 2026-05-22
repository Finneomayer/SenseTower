namespace SC.SenseTower.Common.Models
{
    public class LookupItemDto<TKey>
    {
        public TKey Id { get; set; } = default!;

        public string Name { get; set; } = null!;
    }
}
