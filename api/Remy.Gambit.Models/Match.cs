﻿namespace Remy.Gambit.Models;

public class Match
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int Number { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? Sequence { get; set; }
    public string? WinnerCode { get; set; }
    public IEnumerable<Team>? Teams { get; set; }
    public IEnumerable<string>? Winners { get; set; }
}
