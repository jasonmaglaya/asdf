using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Matches;

namespace Remy.Gambit.Api.Handlers.Matches.Query
{
    public class GetTotalBetsHandler : IQueryHandler<GetTotalBetsRequest, GetTotalBetsResult>
    {
        private readonly IMatchesRepository _matchesRepository;
        private readonly IMapper _mapper;

        public GetTotalBetsHandler(IMatchesRepository matchesRepository, IMapper mapper)
        {
            _matchesRepository = matchesRepository;
            _mapper = mapper;
        }

        public async ValueTask<GetTotalBetsResult> HandleAsync(GetTotalBetsRequest request, CancellationToken token = default)
        {
            var totalBetsAndComm = await _matchesRepository.GetTotalBetsAsync(request.MatchId, token);

            var result = new TotalBetsAndCommission { TotalBets = _mapper.Map<IEnumerable<TotalBet>>(totalBetsAndComm.Item1), Commission = totalBetsAndComm.Item2 };

            return new GetTotalBetsResult { IsSuccessful = true, Result =  result};
        }
    }
}
