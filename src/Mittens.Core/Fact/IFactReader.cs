using Mittens.Core.Fact;
using Mittens.Core.Shared;

namespace Mittens.Core.Fact;

public interface IFactReader
{
    Task<Fact?> GetByIdAsync(int id, CancellationToken ct);
    Task<Fact?> GetByCategoryKeyScopeAsync(string category, string key, string scope, CancellationToken ct);
    Task<PagedResult<Fact>> ListAsync(string? category, string? scope, string? key, int page, int pageSize, CancellationToken ct);
    Task<PagedResult<Fact>> SearchAsync(string q, string? category, string? scope, int page, int pageSize, CancellationToken ct);
}
