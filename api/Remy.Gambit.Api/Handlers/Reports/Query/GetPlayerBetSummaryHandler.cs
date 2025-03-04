using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Reports;

namespace Remy.Gambit.Api.Handlers.Reports.Query;

public class GetPlayerBetSummaryHandler(IReportsRepository reportsRepository, IMapper mapper) : IQueryHandler<GetPlayerBetSummaryRequest, GetPlayerBetSummaryResult>
{
    private readonly IReportsRepository _reportsRepository = reportsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetPlayerBetSummaryResult> HandleAsync(GetPlayerBetSummaryRequest request, CancellationToken token = default)
    {
        var result = await _reportsRepository.GetPlayerBetSummaryAsync(request.MatchId, request.UserId, token);

        return new GetPlayerBetSummaryResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<PlayerBetSummaryItem>>(result) };
    }
}