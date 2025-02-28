using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;

namespace CountryBlockAPI.Core.MediatrHandlers.Country.Commands;

public record UnblockCountryCommand(string CountryCode) : IRequest<ApiResponse<string>>;
public class UnblockCountryCommandValidator : AbstractValidator<UnblockCountryCommand>
{
    public UnblockCountryCommandValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required")
            .Length(2).WithMessage("Country code must be 2 characters")
            .Matches("^[A-Z]{2}$").WithMessage("Country code must be two uppercase letters");
    }
}

public class UnblockCountryHandler(ICountryBlockService service)
    : IRequestHandler<UnblockCountryCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(UnblockCountryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await service.UnblockCountryAsync(request.CountryCode);
            return ApiResponse<string>.Factory.Success(request.CountryCode, $"Country {request.CountryCode} unblocked");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Factory.ServerError(ex.Message);
        }
    }
}