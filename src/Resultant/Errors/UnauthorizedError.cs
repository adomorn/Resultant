namespace Resultant;

public sealed record UnauthorizedError(string Message, string Code) : ResultError(Message, Code);
