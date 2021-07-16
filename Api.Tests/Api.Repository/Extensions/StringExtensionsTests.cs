using Xunit;
using Api.Repository.Extensions;

namespace Api.Tests.Api.Repository.Extensions
{
    public class StringExtensionsTests
    {
        private readonly string _jsonWebToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InN1cGVyLmFkbWluQG15dHJhbnNmb3JtYXRpb24uY29tIiwicm9sZSI6IlN1cGVyQWRtaW4iLCJuYW1laWQiOiI2MGVmODI5ZTZlNTc5MzBkZWI0MWRmZjQiLCJzdGF0aW9uIjoiIiwibmJmIjoxNjI2MzEwODkyLCJleHAiOjE2MjY3NDI4OTIsImlhdCI6MTYyNjMxMDg5Mn0.A6h3nwtufwNHGqqB6JCCKj_j22ztT7r0I-O2dfA3Nt4";

        [Fact]
        public void SelectClaim_ShouldReturnCorrect_ClaimValue()
        {
            const string expectedResult = "super.admin@mytransformation.com";

            string result = _jsonWebToken.SelectClaim("email");

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void SelectClaim_ShouldReturnNull_IfKeyDoesNotExists()
        {
            string result = _jsonWebToken.SelectClaim("dummy");

            Assert.Null(result);
        }

        [Fact]
        public void ClassifyOperation_ShouldReturnCorrect_TypeOperator()
        {
            var query = "FirstName=Isela";

            var result = query.ClassifyOperation();

            Assert.Equal("FirstName", result.Key);
            Assert.Equal("=", result.Operation);
            Assert.Equal("Isela", result.Value);
        }
    }
}