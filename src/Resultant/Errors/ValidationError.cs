namespace Resultant;

public sealed record ValidationError(string Message, string Code, string Property) : ResultError(Message, Code);
