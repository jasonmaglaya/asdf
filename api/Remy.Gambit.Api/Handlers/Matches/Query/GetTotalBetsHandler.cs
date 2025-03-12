using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetTotalBetsHandler(IMatchesRepository matchesRepository, IMapper mapper) : IQueryHandler<GetTotalBetsRequest, GetTotalBetsResult>
    {
        public async ValueTask<GetTotalBetsResult> HandleAsync(GetTotalBetsRequest request, CancellationToken token = default)
        {
            var totalBetsAndComm = await matchesRepository.GetTotalBetsAsync(request.MatchId, token);

            var result = new TotalBetsAndCommission { TotalBets = mapper.Map<IEnumerable<TotalBet>>(totalBetsAndComm.Item1), Commission = totalBetsAndComm.Item2 };

            return new GetTotalBetsResult { IsSuccessful = true, Result =  result};
        }
    }
}
