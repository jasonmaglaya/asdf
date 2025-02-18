namespace Remy.Gambit.Api.Dto
{
    public class Event
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Status { get; set; }
        public string? Video { get; set; }
        public decimal MinimumBet { get; set; }
        public decimal MaximumBet { get; set; }
        public int MaxWinners { get; set; }
        public bool AllowDraw { get; set; }
        public decimal MinDrawBet { get; set; }
        public decimal MaxDrawBet { get; set; }
        public decimal DrawMultiplier { get; set; }
        public bool AllowAgentBetting { get; set; }
        public bool AllowAdminBetting { get; set; }
        public float Commission { get; set; }
        public IEnumerable<Team>? Teams { get; set; }
    }
}
