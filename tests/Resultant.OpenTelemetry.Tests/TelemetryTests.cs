using System.Diagnostics;
using Resultant.OpenTelemetry;

namespace Resultant.OpenTelemetry.Tests;

public class TelemetryTests
{
    private static readonly ActivitySource TestSource = new("Resultant.Test");

    [Fact]
    public void RecordResult_OnSuccess_ShouldSetOkStatus()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = TestSource.StartActivity("test");
        var result = Result.Ok();

        activity.RecordResult(result);

        Assert.Equal("OK", activity?.GetTagItem("otel.status_code")?.ToString());
        Assert.Equal("success", activity?.GetTagItem("result.status")?.ToString());
    }

    [Fact]
    public void RecordResult_OnFailure_ShouldSetErrorStatus()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = TestSource.StartActivity("test");
        var result = Result.Fail(new NotFoundError("Not found", "NF", "User"));

        activity.RecordResult(result);

        Assert.Equal("ERROR", activity?.GetTagItem("otel.status_code")?.ToString());
        Assert.Equal("failure", activity?.GetTagItem("result.status")?.ToString());
        Assert.Equal("NotFoundError", activity?.GetTagItem("result.error.type")?.ToString());
    }

    [Fact]
    public void TapTelemetry_ShouldReturnSameResult()
    {
        var result = Result.Ok(42);
        var tapped = result.TapTelemetry(null);

        Assert.True(tapped.IsSuccess);
        Assert.Equal(42, tapped.Value);
    }

    [Fact]
    public async Task TapTelemetryAsync_ShouldReturnSameResult()
    {
        var result = await Task.FromResult(Result.Ok(42)).TapTelemetryAsync(null);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }
}
