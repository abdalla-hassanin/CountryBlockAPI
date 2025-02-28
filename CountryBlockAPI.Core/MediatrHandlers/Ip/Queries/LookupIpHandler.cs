using System.Net;
using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CountryBlockAPI.Core.MediatrHandlers.Ip.Queries;

public record LookupIpQuery(string? IpAddress) : IRequest<ApiResponse<IpLookupResult>>;
public class LookupIpQueryValidator : AbstractValidator<LookupIpQuery>
{
    public LookupIpQueryValidator()
    {
        RuleFor(x => x.IpAddress)
            .Must(ip => ip == null || IPAddress.TryParse(ip, out _))
            .WithMessage("Invalid IP address format");
    }
}

public class LookupIpHandler(ICountryBlockService service, IHttpContextAccessor httpContext)
    : IRequestHandler<LookupIpQuery, ApiResponse<IpLookupResult>> 
{
    public async Task<ApiResponse<IpLookupResult>> Handle(LookupIpQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var ip = request.IpAddress ?? httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
                return ApiResponse<IpLookupResult>.Factory.BadRequest("Invalid or missing IP address");

            var (countryCode, countryName) = await service.LookupIpAsync(ip);

            if (string.IsNullOrEmpty(countryCode))
                return ApiResponse<IpLookupResult>.Factory.BadRequest("Unable to determine country from IP");

            return ApiResponse<IpLookupResult>.Factory.Success(new IpLookupResult 
            { 
                CountryCode = countryCode, 
                CountryName = countryName 
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<IpLookupResult>.Factory.ServerError(ex.Message);
        }
    }
}