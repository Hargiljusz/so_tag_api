namespace WebAPI.Models
{
    public class PageWrapper<T> where T : class
    {
        public IEnumerable<T> Content { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; } = 0;
        public long TotalPageCount { get; set; }

        public PageWrapper(IEnumerable<T> content, int pageSize, int pageNumber, long totalPageCount)
        {
            Content = content;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalPageCount = totalPageCount;
        }
    }
}
