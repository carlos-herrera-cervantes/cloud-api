namespace Api.Domain.Models
{
    public class Paginator
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int RemainingDocuments { get; set; } = 0;

        public int TotalDocuments { get; set; } = 0;

        /// <summary>
        /// Sets the values for paginator object
        /// </summary>
        /// <param name="request">Request object model</param>
        /// <param name="totalDocuments">Total documents of a collection</param>
        /// <returns>Paginator object model</returns>
        public static Paginator Paginate(ListResourceRequest request, int totalDocuments)
        {
            var (_, pageSize, page, _, _) = request;
            var clonePage = page < 1 ? 1 : page;
            var take = clonePage * pageSize;

            return new Paginator
            {
                Page = clonePage,
                PageSize = pageSize < 1 ? 10 : pageSize,
                RemainingDocuments = (totalDocuments - take) <= 0 ? 0 : totalDocuments - take,
                TotalDocuments = totalDocuments
            };
        }
    }
}
