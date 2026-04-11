namespace Resultant;

public sealed record ForbiddenError(string Message, string Code) : ResultError(Message, Code);
