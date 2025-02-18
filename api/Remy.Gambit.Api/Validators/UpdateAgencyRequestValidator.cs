using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class UpdateAgencyRequestValidator : AbstractValidator<UpdateAgencyRequest>
    {
        public UpdateAgencyRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
            RuleFor(request => request.Requestor).NotNull().WithMessage("Requestor is required");
            RuleFor(request => request.Commission).NotNull().NotEmpty().WithMessage("Commisssion is required")
                .GreaterThan(0).WithMessage("Commisssion mus be greater than 0");
        }
    }
}
