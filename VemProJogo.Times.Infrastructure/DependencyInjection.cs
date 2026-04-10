using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VemProJogo.Times.Application.Abstractions.Persistence;
using VemProJogo.Times.Infrastructure.Configuration;
using VemProJogo.Times.Infrastructure.Persistence;
using VemProJogo.Times.Infrastructure.Persistence.Repositories;

namespace VemProJogo.Times.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(
            configuration.GetSection(MongoDbSettings.SectionName));

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<ITimesRepository, TimesRepository>();
        services.AddScoped<MongoDbInitializer>();

        return services;
    }
}