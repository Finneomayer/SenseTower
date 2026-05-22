using SC.SenseTower.Common.Data;
using System.Text.Json.Serialization;

namespace SC.SenseTower.Common.Models
{
    public class PagedDataRequestDto<TFilter> where TFilter : class, new()
    {
        /// <summary>
        /// Фильтр данных в запросе.
        /// </summary>
        public TFilter Filters { get; set; } = new();

        /// <summary>
        /// Сортировка данных в запросе.
        /// </summary>
        public QuerySorting[] Sorting { get; set; } = Array.Empty<QuerySorting>();

        /// <summary>
        /// Запрашиваемая страница.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Число записей на странице.
        /// </summary>
        public int PageSize { get; set; }

        [JsonIgnore]
        public int Offset => (Page - 1) * PageSize;
    }
}
