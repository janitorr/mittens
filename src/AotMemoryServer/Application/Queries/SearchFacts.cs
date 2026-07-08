using Microsoft.EntityFrameworkCore;
using Mediator;
using AotMemoryServer.Data;
using AotMemoryServer.Models;
using AotMemoryServer.Application.Abstractions;

namespace AotMemoryServer.Application.Queries;

public sealed record SearchFacts(string Q, string? Category, string? Scope, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<MemoryFact>>;

public sealed class SearchFactsHandler(AppDbContext db) : IRequestHandler<SearchFacts, PagedResult<MemoryFact>>
{
    public async ValueTask<PagedResult<MemoryFact>> Handle(SearchFacts query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Max(1, Math.Min(100, query.PageSize));

        var totalCount = await db.Database
            .SqlQueryRaw<int>(MemoryFactSql.SearchFactsCount, query.Q, (object?)query.Category ?? DBNull.Value, (object?)query.Scope ?? DBNull.Value)
            .SingleAsync(cancellationToken);

        var offset = (page - 1) * pageSize;
        var items = await db.MemoryFacts
            .FromSqlRaw(MemoryFactSql.SearchFactsPage, query.Q, (object?)query.Category ?? DBNull.Value, (object?)query.Scope ?? DBNull.Value, pageSize, offset)
            .ToListAsync(cancellationToken);

        return new PagedResult<MemoryFact>(items, totalCount, page, pageSize);
    }
}
