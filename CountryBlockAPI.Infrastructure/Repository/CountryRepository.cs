using System.Collections.Concurrent;
using CountryBlockAPI.Data.Entities;
using CountryBlockAPI.Infrastructure.IRepository;

namespace CountryBlockAPI.Infrastructure.Repository;

public class CountryRepository: ICountryRepository
{
    private readonly ConcurrentDictionary<string, DateTime?> _blockedCountries = new();
    private readonly ConcurrentBag<BlockedAttemptLog> _blockedAttempts = [];

    public Task AddBlockedCountryAsync(string countryCode, DateTime? expiry)
    {
        if (!_blockedCountries.TryAdd(countryCode, expiry))
            throw new InvalidOperationException("Country already blocked");
        return Task.CompletedTask;
    }

    public Task<bool> RemoveBlockedCountryAsync(string countryCode)
    {
        return Task.FromResult(_blockedCountries.TryRemove(countryCode, out _));
    }

    public Task<bool> IsCountryBlockedAsync(string countryCode)
    {
        if (!_blockedCountries.TryGetValue(countryCode, out var expiry)) return Task.FromResult(false);
        if (!expiry.HasValue || expiry.Value >= DateTime.UtcNow) return Task.FromResult(true);
        _blockedCountries.TryRemove(countryCode, out _);
        return Task.FromResult(false);

    }

    public Task<(IEnumerable<string> countries, int totalCount)> GetBlockedCountriesAsync(int page, int size, string filter)
    {
        var filteredCountries = _blockedCountries
            .Where(c => !c.Value.HasValue || c.Value > DateTime.UtcNow)
            .Select(c => c.Key)
            .Where(c => string.IsNullOrEmpty(filter) || c.Contains(filter, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredCountries.ToList();
        var totalCount = enumerable.Count();
        var paginatedCountries = enumerable
            .Skip((page - 1) * size)
            .Take(size);

        return Task.FromResult((paginatedCountries.AsEnumerable(), totalCount));
    }
    public Task AddBlockedAttemptAsync(BlockedAttemptLog log)
    {
        _blockedAttempts.Add(log);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page, int size)
    {
        var logs = _blockedAttempts
            .Where(l => l.IsBlocked)
            .Skip((page - 1) * size)
            .Take(size);
        return Task.FromResult(logs.AsEnumerable());
    }
    public DateTime? GetExpiry(string countryCode) =>
        _blockedCountries.TryGetValue(countryCode, out var expiry) ? expiry : null;

}