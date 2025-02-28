using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;

namespace CountryBlockAPI.Core.MediatrHandlers.Country.Queries;

public record GetBlockedCountriesQuery(int Page, int Size, string Filter) : IRequest<ApiResponse<IEnumerable<string>>>;
public class GetBlockedCountriesQueryValidator : AbstractValidator<GetBlockedCountriesQuery>
{
    public GetBlockedCountriesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100");
    }
}

public class GetBlockedCountriesHandler(ICountryBlockService service)
    : IRequestHandler<GetBlockedCountriesQuery, ApiResponse<IEnumerable<string>>>
{
    public async Task<ApiResponse<IEnumerable<string>>> Handle(GetBlockedCountriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (countries, totalCount) = await service.GetBlockedCountriesAsync(request.Page, request.Size, request.Filter);
            var countriesList = countries.ToList();

            var pagination = new PaginationMetadata
            {
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Size),
                PageSize = request.Size,
                TotalCount = totalCount
            };

            return ApiResponse<IEnumerable<string>>.Factory.WithPagination(countriesList, pagination);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<string>>.Factory.ServerError(ex.Message);
        }
    }
}