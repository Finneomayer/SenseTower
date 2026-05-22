namespace SC.SenseTower.Admin.Dto
{
    public class PaginationDto
    {
        public PaginationDto(int currentPage, int pageSize, long totalCount, int currentCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            TotalCount = totalCount;
            CurrentCount = currentCount;
        }

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public int TotalPages { get; set; } = 0;

        public long TotalCount { get; set; } = 0;

        public int CurrentCount { get; set; } = 0;
    }
}
