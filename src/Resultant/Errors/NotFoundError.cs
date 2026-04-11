namespace Resultant;

public sealed record NotFoundError(string Message, string Code, string Entity) : ResultError(Message, Code);
