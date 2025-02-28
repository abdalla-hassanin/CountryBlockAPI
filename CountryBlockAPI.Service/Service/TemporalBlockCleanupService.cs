using CountryBlockAPI.Infrastructure.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CountryBlockAPI.Service.Service;

public class TemporalBlockCleanupService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICountryRepository>();

                var (countries, _) = await repository.GetBlockedCountriesAsync(1, int.MaxValue, "");
                var countryList = countries.ToList();

                var expiredCountries = new List<string>();
                foreach (var country in countryList)
                {
                    var isBlocked = await repository.IsCountryBlockedAsync(country);
                    if (!isBlocked)
                    {
                        expiredCountries.Add(country);
                    }
                }

                foreach (var country in expiredCountries)
                {
                    await repository.RemoveBlockedCountryAsync(country);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}