using AotMemoryServer.Models;

namespace AotMemoryServer.Application.Serialization;

public sealed record HealthStatus(string Status);

public sealed record ErrorResponse(IReadOnlyList<ValidationError> Errors);
