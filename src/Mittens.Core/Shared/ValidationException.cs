using Mittens.Core.Fact;

namespace Mittens.Core.Shared;

public sealed class ValidationException(IReadOnlyList<ValidationError> errors)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}
