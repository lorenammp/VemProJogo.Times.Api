using Microsoft.Extensions.DependencyInjection;
using VemProJogo.Times.Application.Abstractions.Services;
using VemProJogo.Times.Application.Services;

namespace VemProJogo.Times.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITimesService, TimesService>();
        return services;
    }
}