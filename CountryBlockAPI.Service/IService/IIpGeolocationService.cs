namespace CountryBlockAPI.Service.IService;

public interface IIpGeolocationService
{
    Task<(string CountryCode, string CountryName)> GetCountryFromIpAsync(string ip);
}