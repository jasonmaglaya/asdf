﻿using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Handlers.Credits.Command.Dto;

public class CashOutRequest : ICommand
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    [JsonIgnore]
    public string? IpAddress { get; set; }

    public required string PartnerToken { get; set; }    
}
