using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CountryBlockAPI.Core.MediatrHandlers.Ip.Queries;

public record CheckBlockQuery : IRequest<ApiResponse<BlockCheckResult>>;
public class CheckBlockHandler(
    ICountryBlockService service, IHttpContextAccessor httpContext)
    : IRequestHandler<CheckBlockQuery, ApiResponse<BlockCheckResult>>
{
    public async Task<ApiResponse<BlockCheckResult>> Handle(CheckBlockQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var ip = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
                return ApiResponse<BlockCheckResult>.Factory.BadRequest("Unable to determine IP");

            var userAgent = httpContext.HttpContext?.Request.Headers.UserAgent.ToString() ?? "";
            var result = await service.CheckBlockAsync(ip, userAgent);
            return ApiResponse<BlockCheckResult>.Factory.Success(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<BlockCheckResult>.Factory.ServerError(ex.Message);
        }
    }
}