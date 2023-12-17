namespace Resultant.Tests
{
    public class ResultTests
    {
        [Fact]
        public void Ok_ShouldReturnSuccessResult()
        {
            var result = Result.Ok();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
        }

        [Fact]
        public void Fail_ShouldReturnFailureResult()
        {
            var result = Result.Fail("Error");

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Single(result.Errors);
            Assert.Equal("Error", result.Errors.FirstOrDefault()?.Message);
        }

        [Fact]
        public void Fail_WithMultipleErrors_ShouldReturnAllErrors()
        {
            var errors = new List<Error> { new("Error1"), new("Error2") };
            var result = Result.Fail(errors);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(2, result.Errors.Count());
        }
        [Fact]
        public void ImplicitCastToBool_ShouldBeTrueForSuccess()
        {
            Result result = Result.Ok();
            bool isSuccess = result;

            Assert.True(isSuccess);
        }

        [Fact]
        public void ImplicitCastToBool_ShouldBeFalseForFailure()
        {
            var errors = new List<Error> { new("Error") };
            Result result = Result.Fail(errors);
            bool isFailure = result;

            Assert.False(isFailure);
        }
        [Fact]
        public void ToString_SuccessResult_ShouldReturnSuccess()
        {
            var result = Result.Ok();
            Assert.Equal("Success", result.ToString());
        }

        [Fact]
        public void ToString_FailureResult_ShouldReturnFailureAndError()
        {
            var errors = new List<Error> { new("Error") };
            var result = Result.Fail(errors);
            Assert.Equal("Failure: Error", result.ToString());
        }
    }
}
