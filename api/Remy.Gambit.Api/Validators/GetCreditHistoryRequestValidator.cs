using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Request.Dto;

namespace Remy.Gambit.Api.Validators;

public class GetCreditHistoryRequestValidator : AbstractValidator<GetCreditHistoryRequest>
{
    public GetCreditHistoryRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}
