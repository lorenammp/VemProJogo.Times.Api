using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.Infrastructure.Serialization;

public static class MongoClassMapRegistrar
{
    private static readonly object SyncRoot = new();
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
            return;

        lock (SyncRoot)
        {
            if (_registered)
                return;

            RegisterTimeClassMap();
            _registered = true;
        }
    }

    private static void RegisterTimeClassMap()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Time)))
            return;

        BsonClassMap.RegisterClassMap<Time>(classMap =>
        {
            classMap.AutoMap();
            classMap.SetIgnoreExtraElements(true);

            classMap.MapIdMember(x => x.Id)
                .SetElementName("_id")
                .SetSerializer(new StringSerializer(BsonType.ObjectId))
                .SetIdGenerator(StringObjectIdGenerator.Instance);

            classMap.MapMember(x => x.ChampionshipId)
                .SetElementName("campeonatoId")
                .SetSerializer(new StringSerializer(BsonType.ObjectId));

            classMap.MapMember(x => x.Name)
                .SetElementName("nome");

            classMap.MapMember(x => x.Acronym)
                .SetElementName("sigla");

            classMap.MapMember(x => x.ResponsibleName)
                .SetElementName("responsavelNome");

            classMap.MapMember(x => x.ResponsibleContact)
                .SetElementName("responsavelContato");

            classMap.MapMember(x => x.CrestUrl)
                .SetElementName("escudoUrl");

            classMap.MapMember(x => x.Active)
                .SetElementName("ativo");

            classMap.MapMember(x => x.CreatedAt)
                .SetElementName("criadoEm");

            classMap.MapMember(x => x.UpdatedAt)
                .SetElementName("atualizadoEm");
        });
    }
}