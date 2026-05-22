using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Models
{
    public class BaseListViewModel<T>
    {
        public PaginationDto Pagination { get; set; } = new(1, 10, 0, 0);

        public QuerySorting[] Sorting { get; set; } = Array.Empty<QuerySorting>();

        public T[] Items { get; set; } = Array.Empty<T>();
    }
}
