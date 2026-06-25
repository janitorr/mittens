using AotMemoryServer.Application.Serialization;
using AotMemoryServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AotMemoryServer.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", async (AppDbContext db) =>
        {
            var ok = await db.Database.CanConnectAsync();
            return ok ? Results.Ok(new HealthStatus("healthy")) : Results.StatusCode(503);
        });

        app.MapGet("/api/ready", async (AppDbContext db) =>
        {
            try
            {
                var canConnect = await db.Database.CanConnectAsync();
                if (!canConnect)
                    return Results.StatusCode(503);

                await db.Database.ExecuteSqlRawAsync("SELECT 1");
                return Results.Ok(new HealthStatus("ready"));
            }
            catch
            {
                return Results.StatusCode(503);
            }
        });
    }
}
