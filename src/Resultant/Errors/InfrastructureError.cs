namespace Resultant;

public sealed record InfrastructureError(string Message, string Code, Exception? Inner = null) : ResultError(Message, Code);
