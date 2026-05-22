namespace SC.SenseTower.Common.Data
{
    public class QuerySorting
    {
        /// <summary>
        /// Имя свойства сущности, по которому выполняется сортировка (критерий сортировки).
        /// </summary>
        public string PropertyName { get; set; } = null!;

        /// <summary>
        /// Порядок сортировки в случае нескольких критериев.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Признак сортировки по возрастанию.
        /// </summary>
        public bool Ascending { get; set; } = true;
    }
}
