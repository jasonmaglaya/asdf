namespace Remy.Gambit.Api.Dto
{
    public class TotalBetsAndCommission
    {
        public IEnumerable<TotalBet>? TotalBets { get; set; }
        public decimal? Commission { get; set; }
    }
}
