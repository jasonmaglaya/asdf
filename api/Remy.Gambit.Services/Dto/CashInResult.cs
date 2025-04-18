﻿namespace Remy.Gambit.Services.Dto;

public class CashInResult
{
    public bool IsSuccessful { get; set; }
    public decimal NewBalance { get; set; }
    public string? Currency { get; set; }
    public string[]? Errors{ get; set; }
}
