using Mediator;
using Microsoft.Extensions.Logging;
using Mittens.Core.Fact;
using Mittens.Core.Shared;

namespace Mittens.Core.Fact.Commands;

public sealed record UpsertFact(Fact Fact, bool Force = false) : IRequest<Fact>;

public sealed partial class UpsertFactHandler(IFactReader reader, IFactWriter writer, ILogger<UpsertFactHandler> logger)
    : IRequestHandler<UpsertFact, Fact>
{
    public async ValueTask<Fact> Handle(UpsertFact command, CancellationToken cancellationToken)
    {
        var errors = FactValidator.Validate(command.Fact);
        if (errors.Any(e => !e.IsWarning))
            throw new ValidationException(errors);

        var existing = await reader.GetByCategoryKeyScopeAsync(command.Fact.Category, command.Fact.Key, command.Fact.Scope, cancellationToken);

        if (existing is not null)
        {
            var resolved = FactValidator.ResolveConflict(existing, command.Fact, command.Force);
            if (resolved != existing)
            {
                resolved.UpdatedAt = DateTimeOffset.UtcNow;
                await writer.UpdateAsync(resolved, existing.Id, cancellationToken);
                Log.Upserted(logger, resolved.Category, resolved.Key, resolved.Scope);
                return resolved;
            }

            Log.Upserted(logger, existing.Category, existing.Key, existing.Scope);
            return existing;
        }

        await writer.InsertAsync(command.Fact, cancellationToken);
        Log.Upserted(logger, command.Fact.Category, command.Fact.Key, command.Fact.Scope);
        return command.Fact;
    }

    private static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Upserted fact {Category}/{Key}/{Scope}")]
        public static partial void Upserted(ILogger logger, string category, string key, string scope);
    }
}
