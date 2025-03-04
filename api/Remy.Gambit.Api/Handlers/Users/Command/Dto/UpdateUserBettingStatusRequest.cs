using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Command.Dto;

public class UpdateUserBettingStatusRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public Guid Requestor { get; set; }

    public required bool IsBettingLocked { get; set; }
}
