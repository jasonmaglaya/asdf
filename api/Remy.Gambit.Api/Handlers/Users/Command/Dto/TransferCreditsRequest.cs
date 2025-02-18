using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Users.Command.Dto;

public class TransferCreditsRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public Guid Requestor { get; set; }
    
    public Guid? From { get; set; }
    
    public decimal Amount { get; set; }        
    
    public string? Notes { get; set; }
}
