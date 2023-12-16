namespace Resultant.Tests
{
    public class ResultOfTTests
    {
        [Fact]
        public void Ok_ShouldReturnSuccessResultWithValue()
        {
            var result = Result.Ok(100);

            Assert.True(result.IsSuccess);
            Assert.Equal(100, result.Value);
        }

        [Fact]
        public void Fail_ShouldReturnFailureResult()
        {
            var errors = new List<Error> { new("Error") };
            var result = Result.Fail<int>(errors);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Single(result.Errors);
            Assert.Equal("Error", result.Errors.First().Message);
        }

        [Fact]
        public async Task MapAsync_ShouldMapValueWhenSuccess()
        {
            var result = Result.Ok(10);
            var mappedResult = await result.MapAsync(value => Task.FromResult(value * 2));

            Assert.True(mappedResult.IsSuccess);
            Assert.Equal(20, mappedResult.Value);
        }

        [Fact]
        public async Task BindAsync_ShouldNotBindWhenFailure()
        {
            var errors = new List<Error> { new("Error") };
            var result = Result.Fail<int>(errors);
            var boundResult = await result.BindAsync(value => Task.FromResult(Result.Ok(value * 2)));

            Assert.False(boundResult.IsSuccess);
        }
        [Fact]
        public void ImplicitCastToValueType_ShouldReturnValueForSuccess()
        {
            Result<int> result = Result.Ok(123);
            int value = result;

            Assert.Equal(123, value);
        }

        [Fact]
        public void ImplicitCastToValueType_ShouldThrowForFailure()
        {
            var errors = new List<Error> { new Error("Error") };
            Result<int> result = Result.Fail<int>(errors);

            Assert.Throws<InvalidOperationException>(() => (int)result);
        }

        [Fact]
        public void Map_ShouldTransformValueOnSuccess()
        {
            Result<int> initialResult = Result.Ok(10);
            var mappedResult = initialResult.Map(value => value * 2);

            Assert.True(mappedResult.IsSuccess);
            Assert.Equal(20, mappedResult.Value);
        }

        [Fact]
        public void Map_ShouldNotTransformOnFailure()
        {
            var errors = new List<Error> { new Error("Error") };
            Result<int> initialResult = Result.Fail<int>(errors);
            var mappedResult = initialResult.Map(value => value * 2);

            Assert.False(mappedResult.IsSuccess);
        }

        [Fact]
        public void Bind_ShouldTransformToNewResultOnSuccess()
        {
            Result<int> initialResult = Result.Ok(10);
            var boundResult = initialResult.Bind(value => Result.Ok(value.ToString()));

            Assert.True(boundResult.IsSuccess);
            Assert.Equal("10", boundResult.Value);
        }

        [Fact]
        public void Bind_ShouldNotTransformOnFailure()
        {
            var errors = new List<Error> { new Error("Error") };
            Result<int> initialResult = Result.Fail<int>(errors);
            var boundResult = initialResult.Bind(value => Result.Ok(value.ToString()));

            Assert.False(boundResult.IsSuccess);
        }

        [Fact]
        public async Task MapAsync_ShouldTransformValueOnSuccess()
        {
            Result<int> initialResult = Result.Ok(10);
            var mappedResult = await initialResult.MapAsync(value => Task.FromResult(value * 2));

            Assert.True(mappedResult.IsSuccess);
            Assert.Equal(20, mappedResult.Value);
        }

        [Fact]
        public async Task MapAsync_ShouldNotTransformOnFailure()
        {
            var errors = new List<Error> { new Error("Error") };
            Result<int> initialResult = Result.Fail<int>(errors);
            var mappedResult = await initialResult.MapAsync(value => Task.FromResult(value * 2));

            Assert.False(mappedResult.IsSuccess);
        }
        [Fact]
        public async Task BindAsync_ShouldTransformToNewResultOnSuccess()
        {
            Result<int> initialResult = Result.Ok(10);
            var boundResult = await initialResult.BindAsync(value => Task.FromResult(Result.Ok(value.ToString())));

            Assert.True(boundResult.IsSuccess);
            Assert.Equal("10", boundResult.Value);
        }

        [Fact]
        public async Task BindAsync_ShouldNotTransformOnFailure()
        {
            var errors = new List<Error> { new Error("Error") };
            Result<int> initialResult = Result.Fail<int>(errors);
            var boundResult = await initialResult.BindAsync(value => Task.FromResult(Result.Ok(value.ToString())));

            Assert.False(boundResult.IsSuccess);
        }
    }
}
