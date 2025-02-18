using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto;

public class AdHocLoginRequest : ICommand
{
    public string? Type { get; set; }

    public string? TableId { get; set; }

    public string? UserName { get; set; }

    public string? Currency { get; set; }

    public string? Country { get; set; }

    public string? Lang { get; set; }

    public string? ReloadUrl { get; set; }

    [JsonIgnore]
    public string? ClientId { get; set; }

    [JsonIgnore]
    public string? ClientSecret { get; set; }

    [JsonIgnore]
    public string? IpAddress { get; set; }
}
