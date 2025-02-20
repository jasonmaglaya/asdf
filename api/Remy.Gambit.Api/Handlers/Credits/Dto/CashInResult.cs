﻿using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Credits.Dto;

public class CashInResult : CommandResult
{
    public string? Currency { get; set; }
    public decimal NewBalance { get; set; }
}