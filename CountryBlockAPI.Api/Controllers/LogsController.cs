using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Core.MediatrHandlers.Logs.Queries;
using CountryBlockAPI.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CountryBlockAPI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController(IMediator mediator) : ControllerBase
{
    [HttpGet("blocked-attempts")]
    [SwaggerOperation(Summary = "Get blocked attempt logs")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BlockedAttemptLog>>), StatusCodes.Status200OK)]
    public async Task<IResult> GetBlockedAttempts(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBlockedAttemptsQuery(page, size);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToResult();
    }
}