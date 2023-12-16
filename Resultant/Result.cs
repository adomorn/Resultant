using System.Collections.Generic;
using System.Linq;

namespace Resultant
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public IEnumerable<Error> Errors { get; protected set; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, IEnumerable<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? new List<Error>();
        }

        public static Result Fail(IEnumerable<Error> errors)
        {
            return new Result(false, errors);
        }

        public static Result Fail(string message, int code = 0)
        {
            return Fail(new List<Error> { new Error(message, code) });
        }

        public static Result<T> Fail<T>(IEnumerable<Error> errors)
        {
            return new Result<T>(default, false, errors);
        }

        public static Result Ok()
        {
            return new Result(true, null);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, null);
        }

        public static implicit operator bool(Result result)
        {
            return result.IsSuccess;
        }

        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure: {string.Join(", ", Errors.Select(e => e.Message))}";
        }
    }


}
