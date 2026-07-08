using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AotMemoryServer.Models;

namespace AotMemoryServer.Data;

public static class FactWriter
{
    public static async Task<MemoryFact> InsertAsync(AppDbContext db, MemoryFact fact, CancellationToken ct)
    {
        fact.UpdatedAt = DateTimeOffset.UtcNow;

        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO "MemoryFacts" ("Category", "Key", "Value", "Scope", "Confidence", "Source", "UpdatedAt", "IsDeprecated")
            VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.Add(new SqliteParameter("@p0", fact.Category));
        cmd.Parameters.Add(new SqliteParameter("@p1", fact.Key));
        cmd.Parameters.Add(new SqliteParameter("@p2", fact.Value));
        cmd.Parameters.Add(new SqliteParameter("@p3", fact.Scope));
        cmd.Parameters.Add(new SqliteParameter("@p4", fact.Confidence));
        cmd.Parameters.Add(new SqliteParameter("@p5", fact.Source ?? (object)DBNull.Value));
        cmd.Parameters.Add(new SqliteParameter("@p6", fact.UpdatedAt.ToString("O")));
        cmd.Parameters.Add(new SqliteParameter("@p7", fact.IsDeprecated));

        var id = await cmd.ExecuteScalarAsync(ct);
        fact.Id = id is long l ? (int)l : 0;
        return fact;
    }

    public static async Task UpdateAsync(AppDbContext db, MemoryFact fact, int id, CancellationToken ct)
    {
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE "MemoryFacts" SET "Category" = {fact.Category}, "Key" = {fact.Key}, "Value" = {fact.Value}, "Scope" = {fact.Scope}, "Confidence" = {fact.Confidence}, "Source" = {fact.Source}, "UpdatedAt" = {fact.UpdatedAt}, "IsDeprecated" = {fact.IsDeprecated} WHERE "Id" = {id}
            """, ct);
    }

    public static async Task DeleteAsync(AppDbContext db, int id, CancellationToken ct)
    {
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DELETE FROM "MemoryFacts" WHERE "Id" = {id}
            """, ct);
    }
}
