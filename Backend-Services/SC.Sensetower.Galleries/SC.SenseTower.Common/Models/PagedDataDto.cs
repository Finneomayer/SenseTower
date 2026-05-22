namespace SC.SenseTower.Common.Models
{
    public class PagedDataDto<T>
    {
        /// <summary>
        /// Массив элементов для отображения странице.
        /// </summary>
        public T[] Items { get; set; } = Array.Empty<T>();

        /// <summary>
        /// Общее число элементов в БД, отобранных фильтром.
        /// </summary>
        public long TotalCount { get; set; }
    }
}
