namespace SC.SenseTower.Common.Data
{
    public class BaseListRequest
    {
        public QuerySorting[] Sorting { get; set; } = Array.Empty<QuerySorting>();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
    }
}
