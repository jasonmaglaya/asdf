﻿using FluentValidation;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;

namespace Remy.Gambit.Api.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Old password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long.")
            .Matches(@"[A-Z]")
            .WithMessage("New password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("New password must contain at least one lowercase letter.")
            .Matches(@"[0-9]")
            .WithMessage("New password must contain at least one number.")
            .Matches(@"[\W_]")
            .WithMessage("New password must contain at least one special character.")            ;

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirm password do not match.");
    }
}