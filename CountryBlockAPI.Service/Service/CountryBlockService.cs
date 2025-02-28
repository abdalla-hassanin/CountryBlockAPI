using CountryBlockAPI.Data.Entities;
using CountryBlockAPI.Infrastructure.IRepository;
using CountryBlockAPI.Service.IService;

namespace CountryBlockAPI.Service.Service;

public class CountryBlockService(ICountryRepository countryRepository, IIpGeolocationService geoService)
    : ICountryBlockService
{
    public async Task BlockCountryAsync(string countryCode)
    {
        await countryRepository.AddBlockedCountryAsync(countryCode.ToUpper(), null);
    }

    public async Task UnblockCountryAsync(string countryCode)
    {
        if (!await countryRepository.RemoveBlockedCountryAsync(countryCode.ToUpper()))
            throw new InvalidOperationException("Country not found");
    }

    public async Task TemporalBlockCountryAsync(string countryCode, int durationMinutes)
    {
        if (await countryRepository.IsCountryBlockedAsync(countryCode))
            throw new InvalidOperationException("Country already blocked");

        await countryRepository.AddBlockedCountryAsync(countryCode, DateTime.UtcNow.AddMinutes(durationMinutes));
    }

    public async Task<(IEnumerable<string> countries, int totalCount)> GetBlockedCountriesAsync(int page, int size, string filter)
    {
        if (page < 1 || size < 1)
            throw new ArgumentException("Invalid pagination parameters");

        return await countryRepository.GetBlockedCountriesAsync(page, size, filter.ToUpper());
    }
    public async Task<IEnumerable<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page, int size)
    {
        if (page < 1 || size < 1)
            throw new ArgumentException("Invalid pagination parameters");

        return await countryRepository.GetBlockedAttemptsAsync(page, size);
    }
    public async Task<(string CountryCode, string CountryName)> LookupIpAsync(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress) || !System.Net.IPAddress.TryParse(ipAddress, out _))
            throw new ArgumentException("Invalid IP address");

        return await geoService.GetCountryFromIpAsync(ipAddress);
    }

    public async Task<BlockCheckResult> CheckBlockAsync(string ipAddress, string userAgent)
    {
        if (string.IsNullOrEmpty(ipAddress))
            throw new ArgumentException("IP address is required");

        var (countryCode, countryName) = await geoService.GetCountryFromIpAsync(ipAddress);
        if (string.IsNullOrEmpty(countryCode) && !string.IsNullOrEmpty(countryName))
            throw new InvalidOperationException(countryName); 

        var isBlocked = await countryRepository.IsCountryBlockedAsync(countryCode);
        var log = new BlockedAttemptLog
        {
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow,
            CountryCode = countryCode,
            IsBlocked = isBlocked,
            UserAgent = userAgent
        };
        await countryRepository.AddBlockedAttemptAsync(log);

        return new BlockCheckResult(ipAddress, countryCode, countryName, isBlocked);
    }
}