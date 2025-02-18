using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(request => request.RefreshToken).NotNull().NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
