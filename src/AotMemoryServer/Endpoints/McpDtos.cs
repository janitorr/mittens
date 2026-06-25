using System.Text.Json;
using System.Text.Json.Serialization;

namespace AotMemoryServer.Endpoints;

public sealed record McpRequest(
    [property: JsonPropertyName("jsonrpc")] string JsonRpc,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("params")] JsonElement? Params,
    [property: JsonPropertyName("id")] JsonElement? Id
);

public sealed record McpResponse(
    [property: JsonPropertyName("jsonrpc")] string JsonRpc,
    [property: JsonPropertyName("result")] JsonElement? Result,
    [property: JsonPropertyName("error")] McpError? Error,
    [property: JsonPropertyName("id")] JsonElement? Id
);

public sealed record McpError(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("data")] JsonElement? Data
);
