﻿using Remy.Gambit.Core.Cqs;
using System.Text.Json.Serialization;
namespace Remy.Gambit.Api.Handlers.Events.Query.Dto;

public class GetEventsRequest : IQuery
{
    public string[]? Status { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;    
}
