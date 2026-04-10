using MongoDB.Driver;
using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.Infrastructure.Persistence.Indexes;

public static class TimeIndexesConfigurator
{
    public static async Task ConfigureAsync(
        IMongoCollection<Time> collection,
        CancellationToken cancellationToken = default)
    {
        var uniqueByChampionshipAndName = new CreateIndexModel<Time>(
            Builders<Time>.IndexKeys
                .Ascending(x => x.ChampionshipId)
                .Ascending(x => x.Name),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "uk_times_campeonato_nome"
            });

        var byChampionship = new CreateIndexModel<Time>(
            Builders<Time>.IndexKeys.Ascending(x => x.ChampionshipId),
            new CreateIndexOptions
            {
                Name = "ix_times_campeonatoId"
            });

        var byActive = new CreateIndexModel<Time>(
            Builders<Time>.IndexKeys.Ascending(x => x.Active),
            new CreateIndexOptions
            {
                Name = "ix_times_ativo"
            });

        await collection.Indexes.CreateManyAsync(
            new[] { uniqueByChampionshipAndName, byChampionship, byActive },
            cancellationToken);
    }
}