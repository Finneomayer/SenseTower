using SC.SenseTower.Common.Data;
using System.Text.Json.Serialization;

namespace SC.SenseTower.Admin.Cqrs
{
    public class PagedDataRequest
    {
        public QuerySorting[] Sorting { get; set; } = Array.Empty<QuerySorting>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        [JsonIgnore]
        public int Offset => (Page - 1) * PageSize;
    }

    public class PagedDataRequest<TFilter> : PagedDataRequest where TFilter : class, new()
    {
        public TFilter Filters { get; set; } = new();
    }
}
