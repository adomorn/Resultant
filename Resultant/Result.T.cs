using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resultant
{
    public class Result<T> : Result
    {
        public T Value { get; }

        internal Result(T value, bool isSuccess, IEnumerable<Error> errors) : base(isSuccess, errors)
        {
            Value = value;
        }

        public Result<TNew> Map<TNew>(Func<T, TNew> mapFunc)
        {
            return IsFailure ? Fail<TNew>(Errors) : Success(mapFunc(Value));
        }

        public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> bindFunc)
        {
            return IsFailure ? Fail<TNew>(Errors) : bindFunc(Value);
        }

        public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapFunc)
        {
            if (IsFailure) return Fail<TNew>(Errors);
            var newValue = await mapFunc(Value);
            return Success(newValue);
        }

        public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> bindFunc)
        {
            if (IsFailure) return Fail<TNew>(Errors);
            return await bindFunc(Value);
        }

        public static implicit operator T(Result<T> result)
        {
            if (result.IsFailure)
                throw new InvalidOperationException("Cannot convert a failed result to its value type.");

            return result.Value;
        }
    }
}