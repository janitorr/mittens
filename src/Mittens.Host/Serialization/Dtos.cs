using Mittens.Core.Fact;

namespace Mittens.Serialization;

public sealed record HealthStatus(string Status);

public sealed record ErrorResponse(IReadOnlyList<ValidationError> Errors);
