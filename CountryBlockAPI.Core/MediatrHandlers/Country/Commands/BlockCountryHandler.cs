using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;

namespace CountryBlockAPI.Core.MediatrHandlers.Country.Commands;

public record BlockCountryCommand(string CountryCode) : IRequest<ApiResponse<string>>;

public class BlockCountryCommandValidator : AbstractValidator<BlockCountryCommand>
{
    public BlockCountryCommandValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required")
            .Length(2).WithMessage("Country code must be 2 characters")
            .Matches("^[A-Z]{2}$").WithMessage("Country code must be two uppercase letters");
    }
}

public class BlockCountryHandler(ICountryBlockService countryBlockService)
    : IRequestHandler<BlockCountryCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(BlockCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await countryBlockService.BlockCountryAsync(request.CountryCode);

            return ApiResponse<string>.Factory.Success(
                request.CountryCode,
                $"Country {request.CountryCode} blocked successfully"
            );
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<string>.Factory.BadRequest(
                $"Invalid country code: {ex.Message}"
            );
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<string>.Factory.BadRequest(
                $"Country {request.CountryCode} is already blocked"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Factory.ServerError(
                $"Failed to block country {request.CountryCode}: {ex.Message}"
            );
        }
    }
}