using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
    {
        public UpdateUserRoleRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("User Id is required");
            RuleFor(request => request.RoleCode).NotNull().NotEmpty().WithMessage("Role Code is required");
        }
    }
}
