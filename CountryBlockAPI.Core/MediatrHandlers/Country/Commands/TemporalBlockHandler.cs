using CountryBlockAPI.Core.Base.Response;
using CountryBlockAPI.Service.IService;
using FluentValidation;
using MediatR;

namespace CountryBlockAPI.Core.MediatrHandlers.Country.Commands;

public record TemporalBlockCommand(string CountryCode, int DurationMinutes) : IRequest<ApiResponse<string>>;
public class TemporalBlockCommandValidator : AbstractValidator<TemporalBlockCommand>
{
    public TemporalBlockCommandValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required")
            .Length(2).WithMessage("Country code must be 2 characters")
            .Matches("^[A-Z]{2}$").WithMessage("Country code must be two uppercase letters");
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(1, 1440).WithMessage("Duration must be between 1 and 1440 minutes");
    }
}

public class TemporalBlockHandler(ICountryBlockService service)
    : IRequestHandler<TemporalBlockCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(TemporalBlockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await service.TemporalBlockCountryAsync(request.CountryCode,request.DurationMinutes);
            return ApiResponse<string>.Factory.Success(request.CountryCode, 
                $"Country {request.CountryCode} blocked for {request.DurationMinutes} minutes");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Factory.ServerError(ex.Message);
        }
    }
}