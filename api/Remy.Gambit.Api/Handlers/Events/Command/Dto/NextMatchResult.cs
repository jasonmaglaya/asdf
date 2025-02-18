using Remy.Gambit.Api.Dto;
using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Events.Command.Dto
{
    public class NextMatchResult : CommandResult
    {
        public Match? Match { get; set; }
    }
}
