namespace CountryBlockAPI.Data.Entities;

public class BlockedAttemptLog
{
    public string IpAddress { get; init; }
    public DateTime Timestamp { get; init; }
    public string CountryCode { get; init; }
    public bool IsBlocked { get; init; }
    public string UserAgent { get; init; }
}