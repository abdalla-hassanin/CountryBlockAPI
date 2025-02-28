
using CountryBlockAPI.Data.Entities;

namespace CountryBlockAPI.Service.IService;

public interface ICountryBlockService
{
    Task BlockCountryAsync(string countryCode);
    Task UnblockCountryAsync(string countryCode);
    Task TemporalBlockCountryAsync(string countryCode, int durationMinutes);
    Task<(IEnumerable<string> countries, int totalCount)> GetBlockedCountriesAsync(int page, int size, string filter); 
    Task<IEnumerable<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page, int size); 
    Task<(string CountryCode, string CountryName)> LookupIpAsync(string ipAddress);
    Task<BlockCheckResult> CheckBlockAsync(string ipAddress, string userAgent);
}

public record BlockCheckResult(string Ip, string CountryCode, string CountryName, bool IsBlocked);