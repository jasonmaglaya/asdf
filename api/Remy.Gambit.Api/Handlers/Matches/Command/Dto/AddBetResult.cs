using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Matches.Command.Dto
{
    public class AddBetResult : CommandResult
    {
        public decimal Credits { get; set; }

        public IEnumerable<TotalBet>? Bets { get; set; }
    }
}
