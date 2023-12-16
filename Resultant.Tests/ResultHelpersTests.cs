#pragma warning disable IDE0305
namespace Resultant.Tests
{
    public class ResultHelpersTests
    {
        [Fact]
        public void Combine_WithAllSuccessResults_ShouldReturnSuccess()
        {
            var results = new List<Result>
            {
                Result.Ok(),
                Result.Ok()
            };


            var combinedResult = ResultHelpers.Combine(results.ToArray());


            Assert.True(combinedResult.IsSuccess);
        }

        [Fact]
        public void Combine_WithAnyFailureResults_ShouldReturnFailure()
        {
            var results = new List<Result>
            {
                Result.Ok(),
                Result.Fail("Error")
            };

            var combinedResult = ResultHelpers.Combine(results.ToArray());

            Assert.False(combinedResult.IsSuccess);
            Assert.True(combinedResult.IsFailure);
        }

        [Fact]
        public async Task WhenAll_WithAllSuccessResults_ShouldReturnSuccess()
        {
            var tasks = new List<Task<Result>>
            {
                Task.FromResult(Result.Ok()),
                Task.FromResult(Result.Ok())
            };

            var combinedResult = await ResultHelpers.WhenAll(tasks);

            Assert.True(combinedResult.IsSuccess);
        }

        [Fact]
        public async Task WhenAll_WithAnyFailureResults_ShouldReturnFailure()
        {
            var tasks = new List<Task<Result>>
            {
                Task.FromResult(Result.Ok()),
                Task.FromResult(Result.Fail("Error"))
            };

            var combinedResult = await ResultHelpers.WhenAll(tasks);

            Assert.False(combinedResult.IsSuccess);
            Assert.True(combinedResult.IsFailure);
        }
    }
}
