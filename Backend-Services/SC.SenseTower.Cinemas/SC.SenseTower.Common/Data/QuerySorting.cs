namespace SC.SenseTower.Common.Data
{
    public class QuerySorting
    {
        public string PropertyName { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool Ascending { get; set; } = true;
    }
}
