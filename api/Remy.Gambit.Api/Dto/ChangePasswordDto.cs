﻿namespace Remy.Gambit.Api.Dto;

public class ChangePasswordDto
{
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? SecurityCode { get; set; }
}
