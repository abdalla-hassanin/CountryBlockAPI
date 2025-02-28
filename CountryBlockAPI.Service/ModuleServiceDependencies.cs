using CountryBlockAPI.Service.IService;
using CountryBlockAPI.Service.Service;
using Microsoft.Extensions.DependencyInjection;

namespace CountryBlockAPI.Service;

public static class ModuleServiceDependencies
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<ICountryBlockService, CountryBlockService>();
        services.AddHttpClient<IIpGeolocationService, IpGeolocationService>();
        services.AddHostedService<TemporalBlockCleanupService>();
        return services;
    }
}