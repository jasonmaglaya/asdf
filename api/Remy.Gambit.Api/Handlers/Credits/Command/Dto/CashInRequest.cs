using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Credits.Command.Dto;

public class CashInRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public string? IpAddress { get; set; }

    public required string PartnerToken { get; set; }

    public required decimal Amount { get; set; }

    public string? Currency { get; set; }
}
