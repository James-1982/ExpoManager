using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Expo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Mapster configuration
        var config = TypeAdapterConfig.GlobalSettings;

        MapsterConfig.RegisterMappings();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}