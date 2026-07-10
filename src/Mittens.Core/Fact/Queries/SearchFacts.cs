using Mediator;
using Mittens.Core.Fact;
using Mittens.Core.Shared;

namespace Mittens.Core.Fact.Queries;

public sealed record SearchFacts(string Q, string? Category, string? Scope, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<Fact>>;

public sealed class SearchFactsHandler(IFactReader reader) : IRequestHandler<SearchFacts, PagedResult<Fact>>
{
    public async ValueTask<PagedResult<Fact>> Handle(SearchFacts query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Max(1, Math.Min(100, query.PageSize));

        return await reader.SearchAsync(query.Q, query.Category, query.Scope, page, pageSize, cancellationToken);
    }
}
