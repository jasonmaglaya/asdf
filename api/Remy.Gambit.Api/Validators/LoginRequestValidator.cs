using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(request => request.Username).NotNull().NotEmpty().WithMessage("Username is required");
            RuleFor(request => request.Password).NotNull().NotEmpty().WithMessage("Password is required");
        }
    }
}
