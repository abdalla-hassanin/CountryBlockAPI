using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Core.MediatrHandlers.Ip.Queries;
using CountryBlockAPI.Service.IService;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryBlockAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IpController(IMediator mediator) : ControllerBase
{
    [HttpGet("lookup")]
    [SwaggerOperation(Summary = "Lookup country by IP address")]
    [ProducesResponseType(typeof(ApiResponse<(string CountryCode, string CountryName)>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<(string CountryCode, string CountryName)>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> LookupIp(
        [FromQuery] string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var query = new LookupIpQuery(ipAddress);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToResult();
    }

    [HttpGet("check-block")]
    [SwaggerOperation(Summary = "Check if current IP is blocked")]
    [ProducesResponseType(typeof(ApiResponse<BlockCheckResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BlockCheckResult>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> CheckBlock(
        CancellationToken cancellationToken = default)
    {
        var query = new CheckBlockQuery();
        var result = await mediator.Send(query, cancellationToken);
        return result.ToResult();
    }
}