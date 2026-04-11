using System.Diagnostics.Metrics;

namespace Resultant.OpenTelemetry;

public sealed class ResultMetrics
{
    public const string MeterName = "Resultant";

    private readonly Counter<long> _successCounter;
    private readonly Counter<long> _failureCounter;

    public ResultMetrics(IMeterFactory? meterFactory = null)
    {
        var meter = meterFactory?.Create(MeterName) ?? new Meter(MeterName);
        _successCounter = meter.CreateCounter<long>("resultant.result.success", description: "Number of successful results");
        _failureCounter = meter.CreateCounter<long>("resultant.result.failure", description: "Number of failed results");
    }

    public void RecordOutcome(IResult result)
    {
        if (result.IsSuccess)
        {
            _successCounter.Add(1);
        }
        else
        {
            var errorType = result.FirstError?.GetType().Name ?? "Unknown";
            _failureCounter.Add(1, new KeyValuePair<string, object?>("error.type", errorType));
        }
    }
}
