using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto;

public class ChangePasswordRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public string? Role { get; set; }

    public string? OldPassword { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmPassword { get; set; }

    public string? SecurityCode { get; set; }
}