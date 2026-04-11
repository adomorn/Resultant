namespace Resultant.Tests;

public class LinqQuerySyntaxTests
{
    [Fact]
    public void QuerySyntax_AllSuccess_ShouldReturnCombinedResult()
    {
        var result =
            from x in Result.Ok(1)
            from y in Result.Ok(2)
            select x + y;

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value);
    }

    [Fact]
    public void QuerySyntax_FirstFails_ShouldPropagateFailure()
    {
        var result =
            from x in Result.Fail<int>("First failed")
            from y in Result.Ok(2)
            select x + y;

        Assert.False(result.IsSuccess);
        Assert.Equal("First failed", result.FirstError!.Message);
    }

    [Fact]
    public void QuerySyntax_SecondFails_ShouldPropagateFailure()
    {
        var result =
            from x in Result.Ok(1)
            from y in Result.Fail<int>("Second failed")
            select x + y;

        Assert.False(result.IsSuccess);
        Assert.Equal("Second failed", result.FirstError!.Message);
    }

    [Fact]
    public void QuerySyntax_ThreeResults_ShouldChain()
    {
        var result =
            from x in Result.Ok(1)
            from y in Result.Ok(2)
            from z in Result.Ok(3)
            select x + y + z;

        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value);
    }

    [Fact]
    public void QuerySyntax_WithSelect_ShouldMap()
    {
        var result =
            from x in Result.Ok(10)
            select x * 2;

        Assert.True(result.IsSuccess);
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public void QuerySyntax_WithWhere_MiddleFails_ShouldPropagateError()
    {
        var result =
            from x in Result.Ok(1)
            from y in Result.Fail<int>("Middle failed")
            from z in Result.Ok(3)
            select x + y + z;

        Assert.False(result.IsSuccess);
        Assert.Equal("Middle failed", result.FirstError!.Message);
    }
}
