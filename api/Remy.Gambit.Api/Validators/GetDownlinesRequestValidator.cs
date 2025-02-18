using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;

namespace Remy.Gambit.Api.Validators
{
    public class GetDownlinesRequestValidator : AbstractValidator<GetDownLinesRequest>
    {
        public GetDownlinesRequestValidator()
        {
            RuleFor(request => request.UserId).NotNull().WithMessage("UserId is required");
        }
    }
}
