using MediatR;

namespace Resultant.MediatR;

public interface IResultRequest : IRequest<Result> { }

public interface IResultRequest<T> : IRequest<Result<T>> { }
