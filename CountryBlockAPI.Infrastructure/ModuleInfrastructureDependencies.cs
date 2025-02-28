using CountryBlockAPI.Infrastructure.IRepository;
using CountryBlockAPI.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace CountryBlockAPI.Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static void AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddSingleton<ICountryRepository, CountryRepository>();
    }
}