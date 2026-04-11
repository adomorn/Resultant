namespace Resultant;

public sealed record ConflictError(string Message, string Code) : ResultError(Message, Code);
