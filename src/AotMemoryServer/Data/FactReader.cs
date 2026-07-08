using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AotMemoryServer.Models;
using AotMemoryServer.Application.Abstractions;

namespace AotMemoryServer.Data;

public static class FactReader
{
    public static async Task<MemoryFact?> GetByIdAsync(AppDbContext db, int id, CancellationToken ct)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = MemoryFactSql.GetById;
        cmd.Parameters.Add(new SqliteParameter("@p0", id));

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? MapFact(reader) : null;
    }

    public static async Task<MemoryFact?> GetByCategoryKeyScopeAsync(AppDbContext db, string category, string key, string scope, CancellationToken ct)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = MemoryFactSql.GetByCategoryKeyScope;
        cmd.Parameters.Add(new SqliteParameter("@p0", category));
        cmd.Parameters.Add(new SqliteParameter("@p1", key));
        cmd.Parameters.Add(new SqliteParameter("@p2", scope));

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? MapFact(reader) : null;
    }

    public static async Task<PagedResult<MemoryFact>> ListAsync(AppDbContext db, string? category, string? scope, string? key, int page, int pageSize, CancellationToken ct)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        var offset = (page - 1) * pageSize;

        await using var countCmd = conn.CreateCommand();
        countCmd.CommandText = MemoryFactSql.GetFactsCount;
        countCmd.Parameters.Add(new SqliteParameter("@p0", (object?)category ?? DBNull.Value));
        countCmd.Parameters.Add(new SqliteParameter("@p1", (object?)scope ?? DBNull.Value));
        countCmd.Parameters.Add(new SqliteParameter("@p2", (object?)key ?? DBNull.Value));
        var totalCount = (int)(long)(await countCmd.ExecuteScalarAsync(ct))!;

        await using var itemsCmd = conn.CreateCommand();
        itemsCmd.CommandText = MemoryFactSql.GetFactsPage;
        itemsCmd.Parameters.Add(new SqliteParameter("@p0", (object?)category ?? DBNull.Value));
        itemsCmd.Parameters.Add(new SqliteParameter("@p1", (object?)scope ?? DBNull.Value));
        itemsCmd.Parameters.Add(new SqliteParameter("@p2", (object?)key ?? DBNull.Value));
        itemsCmd.Parameters.Add(new SqliteParameter("@p3", pageSize));
        itemsCmd.Parameters.Add(new SqliteParameter("@p4", offset));

        var items = new List<MemoryFact>();
        await using var reader = await itemsCmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            items.Add(MapFact(reader));

        return new PagedResult<MemoryFact>(items, totalCount, page, pageSize);
    }

    public static async Task<PagedResult<MemoryFact>> SearchAsync(AppDbContext db, string q, string? category, string? scope, int page, int pageSize, CancellationToken ct)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        var offset = (page - 1) * pageSize;

        await using var countCmd = conn.CreateCommand();
        countCmd.CommandText = MemoryFactSql.SearchFactsCount;
        countCmd.Parameters.Add(new SqliteParameter("@p0", q));
        countCmd.Parameters.Add(new SqliteParameter("@p1", (object?)category ?? DBNull.Value));
        countCmd.Parameters.Add(new SqliteParameter("@p2", (object?)scope ?? DBNull.Value));
        var totalCount = (int)(long)(await countCmd.ExecuteScalarAsync(ct))!;

        await using var itemsCmd = conn.CreateCommand();
        itemsCmd.CommandText = MemoryFactSql.SearchFactsPage;
        itemsCmd.Parameters.Add(new SqliteParameter("@p0", q));
        itemsCmd.Parameters.Add(new SqliteParameter("@p1", (object?)category ?? DBNull.Value));
        itemsCmd.Parameters.Add(new SqliteParameter("@p2", (object?)scope ?? DBNull.Value));
        itemsCmd.Parameters.Add(new SqliteParameter("@p3", pageSize));
        itemsCmd.Parameters.Add(new SqliteParameter("@p4", offset));

        var items = new List<MemoryFact>();
        await using var reader = await itemsCmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            items.Add(MapFact(reader));

        return new PagedResult<MemoryFact>(items, totalCount, page, pageSize);
    }

    private static MemoryFact MapFact(DbDataReader reader)
    {
        return new MemoryFact
        {
            Id = reader.GetInt32(0),
            Category = reader.GetString(1),
            Key = reader.GetString(2),
            Value = reader.GetString(3),
            Scope = reader.GetString(4),
            Confidence = reader.GetDouble(5),
            Source = reader.IsDBNull(6) ? null : reader.GetString(6),
            UpdatedAt = DateTimeOffset.Parse(reader.GetString(7), CultureInfo.InvariantCulture),
            IsDeprecated = reader.GetBoolean(8),
        };
    }
}
