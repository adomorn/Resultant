using System.Diagnostics;

namespace Resultant.OpenTelemetry;

public static class ResultTelemetryExtensions
{
    public static void RecordResult(this Activity? activity, IResult result)
    {
        if (activity == null) return;

        if (result.IsSuccess)
        {
            activity.SetTag("otel.status_code", "OK");
            activity.SetTag("result.status", "success");
        }
        else
        {
            activity.SetTag("otel.status_code", "ERROR");
            activity.SetTag("result.status", "failure");

            if (result.FirstError != null)
            {
                activity.SetTag("result.error.type", result.FirstError.GetType().Name);
                activity.SetTag("result.error.code", result.FirstError.Code);
                activity.SetTag("result.error.message", result.FirstError.Message);
            }

            activity.SetTag("result.error.count", result.Errors.Count);
        }
    }

    public static Result<T> TapTelemetry<T>(this Result<T> result, Activity? activity)
    {
        activity.RecordResult(result);
        return result;
    }

    public static async Task<Result<T>> TapTelemetryAsync<T>(this Task<Result<T>> resultTask, Activity? activity)
    {
        var result = await resultTask;
        activity.RecordResult(result);
        return result;
    }
}
