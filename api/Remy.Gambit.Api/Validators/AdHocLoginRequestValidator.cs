using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class AdHocLoginRequestValidator : AbstractValidator<AdHocLoginRequest>
    {
        public AdHocLoginRequestValidator()
        {
            //RuleFor(request => request.ClientId).NotNull().NotEmpty().WithMessage("ClientId is required");
            //RuleFor(request => request.ClientSecret).NotNull().NotEmpty().WithMessage("ClientSecret is required");
            RuleFor(request => request.Type).NotNull().NotEmpty().WithMessage("Type is required");
            RuleFor(request => request.UserName).NotNull().NotEmpty().WithMessage("UserName is required");
        }
    }
}
