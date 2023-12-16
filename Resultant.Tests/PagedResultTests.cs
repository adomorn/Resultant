namespace Resultant.Tests
{
    public class PagedResultTests
    {
        [Fact]
        public void Create_ShouldReturnSuccessPagedResult()
        {
            var items = new List<int> { 1, 2, 3 };
            var pagedResult = PagedResult<int>.Create(items, 1, 3, 10);

            Assert.True(pagedResult.IsSuccess);
            Assert.Equal(items, pagedResult.Value);
            Assert.Equal(1, pagedResult.CurrentPage);
            Assert.Equal(3, pagedResult.PageSize);
            Assert.Equal(10, pagedResult.TotalCount);
            Assert.Equal(4, pagedResult.TotalPages);
        }

        [Fact]
        public void Fail_ShouldReturnFailurePagedResult()
        {
            var pagedResult = PagedResult<int>.Fail("Error");

            Assert.False(pagedResult.IsSuccess);
            Assert.True(pagedResult.IsFailure);
        }
    }
}
