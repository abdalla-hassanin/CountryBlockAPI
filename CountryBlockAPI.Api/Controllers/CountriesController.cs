using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Core.MediatrHandlers.Country.Commands;
using CountryBlockAPI.Core.MediatrHandlers.Country.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryBlockAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(IMediator mediator) : ControllerBase
{
    [HttpPost("block")]
    [SwaggerOperation(Summary = "Block a country permanently")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> BlockCountry(
        [FromBody] BlockCountryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToResult();
    }

    [HttpDelete("block/{countryCode}")]
    [SwaggerOperation(Summary = "Unblock a country")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IResult> UnblockCountry(
        string countryCode,
        CancellationToken cancellationToken)
    {
        var command = new UnblockCountryCommand(countryCode);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToResult();
    }

    [HttpGet("blocked")]
    [SwaggerOperation(Summary = "Get list of blocked countries")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<IResult> GetBlockedCountries(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string filter = "",
        CancellationToken cancellationToken = default)
    {
        var query = new GetBlockedCountriesQuery(page, size, filter);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToResult();
    }

    [HttpPost("temporal-block")]
    [SwaggerOperation(Summary = "Block a country temporarily")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IResult> TemporalBlock(
        [FromBody] TemporalBlockCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToResult();
    }
}