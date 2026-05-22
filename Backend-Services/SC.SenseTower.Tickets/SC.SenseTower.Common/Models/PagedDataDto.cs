namespace SC.SenseTower.Common.Models
{
    public class PagedDataDto<T>
    {
        public T[] Items { get; set; } = Array.Empty<T>();

        public long TotalCount { get; set; }
    }
}
