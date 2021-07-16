using Xunit;
using Api.Repository.Extensions;

namespace Api.Tests.Api.Repository.Extensions
{
    public class ArrayExtensionsTests
    {
        [Fact]
        public void IsEmpty_Should_Return_True()
        {
            var arr = new int[0];

            bool result = arr.IsEmpty();

            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_Should_Return_False()
        {
            var arr = new [] { "carlos" };

            bool result = arr.IsEmpty();

            Assert.False(result);
        }
    }
}