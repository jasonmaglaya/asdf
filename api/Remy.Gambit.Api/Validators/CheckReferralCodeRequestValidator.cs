using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Query.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class CheckReferralCodeRequestValidator : AbstractValidator<CheckReferralCodeRequest>
    {
        public CheckReferralCodeRequestValidator()
        {
            RuleFor(request => request.ReferralCode).NotNull().NotEmpty().WithMessage("Referral Code is required");
        }
    }
}
