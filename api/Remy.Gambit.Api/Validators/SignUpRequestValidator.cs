using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(request => request.Username)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required")
                .Length(5, 20)
                .WithMessage("Username length must between 5 to 20 characters");

            RuleFor(request => request.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password is required")
                .WithMessage("Invalid password value");

            RuleFor(request => request.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .WithMessage("Confirm Password is required")
                .Equal(x => x.Password)
                .WithMessage("Confirm Password does not match");

            RuleFor(request => request.ContactNumber)
                .NotNull()
                .NotEmpty()
                .WithMessage("Contact Number is required")
                .Length(8, 15)
                .WithMessage("Contact Number length must between 8 to 15 characters");
        }
    }
}
