using Mediator;
using Microsoft.Extensions.Logging;
using Mittens.Core.Fact;
using Mittens.Core.Shared;

namespace Mittens.Core.Fact.Commands;

public sealed record UpdateFact(int Id, Fact Fact) : IRequest<Fact?>;

public sealed partial class UpdateFactHandler(IFactReader reader, IFactWriter writer, ILogger<UpdateFactHandler> logger)
    : IRequestHandler<UpdateFact, Fact?>
{
    public async ValueTask<Fact?> Handle(UpdateFact command, CancellationToken cancellationToken)
    {
        var errors = FactValidator.Validate(command.Fact);
        if (errors.Any(e => !e.IsWarning))
            throw new ValidationException(errors);

        var existing = await reader.GetByIdAsync(command.Id, cancellationToken);

        if (existing is null)
            return null;

        command.Fact.Id = command.Id;
        command.Fact.UpdatedAt = DateTimeOffset.UtcNow;

        await writer.UpdateAsync(command.Fact, command.Id, cancellationToken);

        Log.Updated(logger, command.Id, command.Fact.Category, command.Fact.Key, command.Fact.Scope);
        return command.Fact;
    }

    private static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Updated fact {Id} ({Category}/{Key}/{Scope})")]
        public static partial void Updated(ILogger logger, int id, string category, string key, string scope);
    }
}
