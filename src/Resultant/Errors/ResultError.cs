namespace Resultant;

/// <summary>
/// Base error type for all Result errors.
/// </summary>
public abstract record ResultError(string Message, string Code);
