using Mittens.Core.Fact;

namespace Mittens.Core.Fact;

public interface IFactWriter
{
    Task<Fact> InsertAsync(Fact fact, CancellationToken ct);
    Task UpdateAsync(Fact fact, int id, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
