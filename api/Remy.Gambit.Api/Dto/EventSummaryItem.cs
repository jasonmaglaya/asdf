﻿namespace Remy.Gambit.Api.Dto;

public class EventSummaryItem
{
    public int Number { get; set; }
    public string? BetOn { get; set; }
    public decimal Bet { get; set; }
    public DateTime BetTimeStamp { get; set; }
    public string? Winners { get; set; }
    public decimal GainLoss { get; set; }
    public DateTime GainLossDate { get; set; }
    public string? Notes { get; set; }
}
