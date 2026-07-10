using Mediator;
using Mittens.Core.Fact;
using Mittens.Core.Shared;

namespace Mittens.Core.Fact.Queries;

public sealed record GetFacts(string? Category, string? Scope, string? Key, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<Fact>>;

public sealed class GetFactsHandler(IFactReader reader) : IRequestHandler<GetFacts, PagedResult<Fact>>
{
    public async ValueTask<PagedResult<Fact>> Handle(GetFacts query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Max(1, Math.Min(100, query.PageSize));

        return await reader.ListAsync(query.Category, query.Scope, query.Key, page, pageSize, cancellationToken);
    }
}
