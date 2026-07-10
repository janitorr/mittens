namespace Mittens.Core.Fact;

public sealed record ValidationError(string Property, string Message, bool IsWarning = false);
