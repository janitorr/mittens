using Mediator;
using Mittens.Core.Fact;

namespace Mittens.Core.Fact.Queries;

public sealed record GetFactById(int Id) : IRequest<Fact?>;

public sealed class GetFactByIdHandler(IFactReader reader) : IRequestHandler<GetFactById, Fact?>
{
    public async ValueTask<Fact?> Handle(GetFactById query, CancellationToken cancellationToken)
    {
        return await reader.GetByIdAsync(query.Id, cancellationToken);
    }
}
