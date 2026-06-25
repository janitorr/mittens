using System.Text.Json;
using System.Text.Json.Nodes;
using AotMemoryServer.Models;
using AotMemoryServer.Application.Abstractions;
using AotMemoryServer.Application.Commands;
using AotMemoryServer.Application.Queries;

namespace AotMemoryServer.Endpoints;

public static class McpEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    public static void MapMcpEndpoints(this WebApplication app)
    {
        app.MapPost("/api/mcp", async (HttpContext context) =>
        {
            JsonNode? body;
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var text = await reader.ReadToEndAsync();
                body = JsonNode.Parse(text);
            }
            catch (JsonException)
            {
                return Error(null, -32700, "Parse error");
            }

            if (body is not JsonObject json)
                return Error(null, -32700, "Parse error");

            if (json["jsonrpc"]?.GetValue<string>() != "2.0" || json["method"]?.GetValue<string>() is not { } method)
                return Error(json["id"], -32600, "Invalid Request");

            var id = json["id"];

            try
            {
                var result = method switch
                {
                    "memory/list" => await HandleList(context, json["params"]),
                    "memory/get" => await HandleGet(context, json["params"]),
                    "memory/search" => await HandleSearch(context, json["params"]),
                    "memory/set" => await HandleSet(context, json["params"]),
                    "memory/update" => await HandleUpdate(context, json["params"]),
                    "memory/delete" => await HandleDelete(context, json["params"]),
                    _ => throw new KeyNotFoundException()
                };

                if (id is null)
                    return Results.Empty;

                return Result(id, result);
            }
            catch (KeyNotFoundException)
            {
                return Error(id, -32601, $"Method not found: {method}");
            }
            catch (ValidationException ex)
            {
                return Error(id, -32000, "Validation error", new { errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return Error(id, -32603, "Internal error", new { message = ex.Message });
            }
        });
    }

    private static async Task<object?> HandleList(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<IQueryHandler<GetFacts, PagedResult<MemoryFact>>>();
        var query = new GetFacts(
            p?["category"]?.GetValue<string>(),
            p?["scope"]?.GetValue<string>(),
            p?["key"]?.GetValue<string>(),
            p?["page"]?.GetValue<int>() ?? 1,
            p?["pageSize"]?.GetValue<int>() ?? 20
        );
        return await handler.Handle(query);
    }

    private static async Task<object?> HandleGet(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<IQueryHandler<GetFactById, MemoryFact?>>();
        var id = p?["id"]?.GetValue<int>() ?? 0;
        return await handler.Handle(new GetFactById(id));
    }

    private static async Task<object?> HandleSearch(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<IQueryHandler<SearchFacts, PagedResult<MemoryFact>>>();
        var query = new SearchFacts(
            p?["q"]?.GetValue<string>() ?? string.Empty,
            p?["category"]?.GetValue<string>(),
            p?["scope"]?.GetValue<string>(),
            p?["page"]?.GetValue<int>() ?? 1,
            p?["pageSize"]?.GetValue<int>() ?? 20
        );
        return await handler.Handle(query);
    }

    private static async Task<object?> HandleSet(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<ICommandHandler<UpsertFact, MemoryFact>>();
        var factNode = p?["fact"];
        var fact = factNode.Deserialize<MemoryFact>(JsonOptions)
            ?? throw new ValidationException([new ValidationError("fact", "Fact is required.")]);
        var force = p?["force"]?.GetValue<bool>() ?? false;
        return await handler.Handle(new UpsertFact(fact, force));
    }

    private static async Task<object?> HandleUpdate(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<ICommandHandler<UpdateFact, MemoryFact?>>();
        var id = p?["id"]?.GetValue<int>() ?? 0;
        var factNode = p?["fact"];
        var fact = factNode.Deserialize<MemoryFact>(JsonOptions)
            ?? throw new ValidationException([new ValidationError("fact", "Fact is required.")]);
        return await handler.Handle(new UpdateFact(id, fact));
    }

    private static async Task<object?> HandleDelete(HttpContext context, JsonNode? paramsNode)
    {
        var p = paramsNode as JsonObject;
        var handler = context.RequestServices.GetRequiredService<ICommandHandler<DeleteFact, bool>>();
        var id = p?["id"]?.GetValue<int>() ?? 0;
        return await handler.Handle(new DeleteFact(id));
    }

    private static IResult Result(JsonNode? id, object? result)
    {
        return Results.Json(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["result"] = JsonSerializer.SerializeToNode(result),
            ["id"] = id?.DeepClone()
        });
    }

    private static IResult Error(JsonNode? id, int code, string message, object? data = null)
    {
        var error = new JsonObject
        {
            ["code"] = code,
            ["message"] = message
        };
        if (data is not null)
            error["data"] = JsonSerializer.SerializeToNode(data);

        return Results.Json(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["error"] = error,
            ["id"] = id?.DeepClone()
        });
    }
}
