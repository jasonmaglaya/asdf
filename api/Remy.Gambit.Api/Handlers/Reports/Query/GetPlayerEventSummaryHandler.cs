using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Reports;

namespace Remy.Gambit.Api.Handlers.Reports.Query;

public class GetPlayerEventSummaryHandler(IReportsRepository reportsRepository, IMapper mapper) : IQueryHandler<GetPlayerEventSummaryRequest, GetPlayerEventSummaryResult>
{
    private readonly IReportsRepository _reportsRepository = reportsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetPlayerEventSummaryResult> HandleAsync(GetPlayerEventSummaryRequest request, CancellationToken token = default)
    {
        var result = await _reportsRepository.GetPlayerEventSummaryAsync(request.EventId, request.UserId, token);

        return new GetPlayerEventSummaryResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<PlayerEventSummaryItem>>(result) };
    }
}