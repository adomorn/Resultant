using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resultant
{
    public static class ResultHelpers
    {
        public static Result Combine(params Result[] results)
        {
            var failedResults = results.Where(r => r.IsFailure).ToList();
            return failedResults.Any() ? Result.Fail(failedResults.SelectMany(r => r.Errors)) : Result.Success();
        }

        public static async Task<Result> WhenAll(IEnumerable<Task<Result>> tasks)
        {
            var results = await Task.WhenAll(tasks);
            return Combine(results);
        }
    }
}