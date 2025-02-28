using CountryBlockAPI.Service.IService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CountryBlockAPI.Service.Service;

public class IpGeolocationService : IIpGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public IpGeolocationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["IpGeolocation:ApiKey"] ?? throw new InvalidOperationException("IPGeolocation API key is missing");
        _httpClient.BaseAddress = new Uri("https://api.ipgeolocation.io/");
    }

    public async Task<(string CountryCode, string CountryName)> GetCountryFromIpAsync(string ip)
    {
        if (!System.Net.IPAddress.TryParse(ip, out var ipAddress))
            return ("", "Invalid IP address format");

        if (System.Net.IPAddress.IsLoopback(ipAddress) || ip.StartsWith("0."))
            return ("N/A", "Reserved or Localhost");

        try
        {
            var url = $"ipgeo?apiKey={_apiKey}&ip={ip}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Locked => ("", "Account locked. Check IPGeolocation.io status."),
                    System.Net.HttpStatusCode.TooManyRequests => ("", "Too many requests. Try again later."),
                    _ => ("", $"Error from geolocation service: {response.StatusCode} - {errorContent}")
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            return ((string)data.country_code2, (string)data.country_name);
        }
        catch (Exception ex)
        {
            return ("", $"Error processing geolocation request: {ex.Message}");
        }
    }
}