using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Command.Dto;

public class UpdateUserRoleRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public required string RoleCode { get; set; }
}
