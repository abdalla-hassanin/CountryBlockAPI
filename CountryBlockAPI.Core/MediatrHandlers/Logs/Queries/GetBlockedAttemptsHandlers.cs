using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Data.Entities;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;

namespace CountryBlockAPI.Core.MediatrHandlers.Logs.Queries;

public record GetBlockedAttemptsQuery(int Page, int Size) : IRequest<ApiResponse<IEnumerable<BlockedAttemptLog>>>;

public class GetBlockedAttemptsQueryValidator : AbstractValidator<GetBlockedAttemptsQuery>
{
    public GetBlockedAttemptsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100");
    }
}

public class GetBlockedAttemptsHandler(ICountryBlockService service)
    : IRequestHandler<GetBlockedAttemptsQuery, ApiResponse<IEnumerable<BlockedAttemptLog>>>
{
    public async Task<ApiResponse<IEnumerable<BlockedAttemptLog>>> Handle(GetBlockedAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var logs = (await service.GetBlockedAttemptsAsync(request.Page, request.Size)).ToList();
            var totalCount = logs.Count;
            var pagination = new PaginationMetadata
            {
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Size),
                PageSize = request.Size,
                TotalCount = totalCount
            };
            return ApiResponse<IEnumerable<BlockedAttemptLog>>.Factory.WithPagination(logs, pagination);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<BlockedAttemptLog>>.Factory.ServerError(ex.Message);
        }
    }
}