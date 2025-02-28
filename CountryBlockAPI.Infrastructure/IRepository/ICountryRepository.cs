using CountryBlockAPI.Data.Entities;

namespace CountryBlockAPI.Infrastructure.IRepository;

public interface ICountryRepository
{
    Task AddBlockedCountryAsync(string countryCode, DateTime? expiry);
    Task<bool> RemoveBlockedCountryAsync(string countryCode);
    Task<bool> IsCountryBlockedAsync(string countryCode);
    Task<(IEnumerable<string> countries, int totalCount)> GetBlockedCountriesAsync(int page, int size, string filter);
    Task AddBlockedAttemptAsync(BlockedAttemptLog log);
    Task<IEnumerable<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page, int size);
}