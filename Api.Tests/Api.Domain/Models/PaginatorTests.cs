using Xunit;
using Api.Domain.Models;

namespace Api.Tests.Api.Domain.Models
{
    public class PaginatorTests
    {
        [Fact]
        public void Paginate_ShouldReturn_CorrectPagePageSize()
        {
            const int totalDocs = 100;
            var request = new ListResourceRequest
            {
                Page = 1,
                PageSize = 20
            };
            var expectedResult = new Paginator
            {
                Page = 1,
                PageSize = 20,
                RemainingDocuments = 80,
                TotalDocuments = totalDocs
            };

            Paginator result = Paginator.Paginate(request, totalDocs);

            Assert.IsType<Paginator>(result);
            Assert.Equal(result.Page, expectedResult.Page);
            Assert.Equal(result.PageSize, expectedResult.PageSize);
            Assert.Equal(result.RemainingDocuments, expectedResult.RemainingDocuments);
            Assert.Equal(result.TotalDocuments, expectedResult.TotalDocuments);
        }
    }
}