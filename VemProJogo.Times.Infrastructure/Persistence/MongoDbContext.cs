using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VemProJogo.Times.Domain.Constants;
using VemProJogo.Times.Domain.Entities;
using VemProJogo.Times.Infrastructure.Configuration;
using VemProJogo.Times.Infrastructure.Serialization;

namespace VemProJogo.Times.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    public IMongoDatabase Database { get; }
    public IMongoCollection<Time> Times { get; }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException("MongoDb:ConnectionString não configurada.");

        if (string.IsNullOrWhiteSpace(settings.DatabaseName))
            throw new InvalidOperationException("MongoDb:DatabaseName não configurado.");

        MongoClassMapRegistrar.Register();

        var client = new MongoClient(settings.ConnectionString.Trim());
        Database = client.GetDatabase(settings.DatabaseName.Trim());
        Times = Database.GetCollection<Time>(CollectionNames.Times);
    }
}