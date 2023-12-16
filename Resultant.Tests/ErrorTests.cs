namespace Resultant.Tests
{
    using Xunit;

    namespace Resultant.Tests
    {
        public class ErrorTests
        {
            [Fact]
            public void Error_ShouldSetMessage()
            {
                var errorMessage = "Test Error";
                var error = new Error(errorMessage);

                Assert.Equal(errorMessage, error.Message);
            }

            [Fact]
            public void Error_ShouldSetDefaultCodeToZero()
            {
                var error = new Error("Test Error");

                Assert.Equal(0, error.Code);
            }

            [Theory]
            [InlineData(100)]
            [InlineData(404)]
            [InlineData(500)]
            public void Error_ShouldSetCustomCode(int errorCode)
            {
                var error = new Error("Test Error", errorCode);

                Assert.Equal(errorCode, error.Code);
            }
        }
    }

}
