using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using VemProJogo.Times.Infrastructure.Persistence.Indexes;

namespace VemProJogo.Times.Infrastructure.Persistence;

public sealed class MongoDbInitializer
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoDbInitializer> _logger;

    public MongoDbInitializer(
        MongoDbContext context,
        ILogger<MongoDbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Inicializando MongoDB do microsserviço de Times.");

        await _context.Database.RunCommandAsync<BsonDocument>(
            new BsonDocument("ping", 1),
            cancellationToken: cancellationToken);

        await TimeIndexesConfigurator.ConfigureAsync(_context.Times, cancellationToken);

        _logger.LogInformation("MongoDB do microsserviço de Times inicializado com sucesso.");
    }
}