using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Reports;

namespace Remy.Gambit.Api.Handlers.Reports.Query;

public class GetMatchSummaryHandler(IReportsRepository reportsRepository, IMapper mapper) : IQueryHandler<GetMatchSummaryRequest, GetMatchSummaryResult>
{
    private readonly IReportsRepository _reportsRepository = reportsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetMatchSummaryResult> HandleAsync(GetMatchSummaryRequest request, CancellationToken token = default)
    {
        var result = await _reportsRepository.GetMatchSummaryAsync(request.MatchId, token);

        return new GetMatchSummaryResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<MatchSummaryItem>>(result) };
    }
}