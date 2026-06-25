using System.Text.Json.Serialization;
using AotMemoryServer.Application.Abstractions;
using AotMemoryServer.Endpoints;
using AotMemoryServer.Models;

namespace AotMemoryServer.Application.Serialization;

[JsonSerializable(typeof(MemoryFact))]
[JsonSerializable(typeof(PagedResult<MemoryFact>))]
[JsonSerializable(typeof(List<MemoryFact>))]
[JsonSerializable(typeof(ValidationError))]
[JsonSerializable(typeof(HealthStatus))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(McpRequest))]
[JsonSerializable(typeof(McpResponse))]
[JsonSerializable(typeof(McpError))]
public sealed partial class AppJsonContext : JsonSerializerContext
{
}
