using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Reports;

namespace Remy.Gambit.Api.Handlers.Events.Query;

public class GetEventSummaryHandler(IReportsRepository reportsRepository, IMapper mapper) : IQueryHandler<GetEventSummaryRequest, GetEventSummaryResult>
{
    private readonly IReportsRepository _reportsRepository = reportsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetEventSummaryResult> HandleAsync(GetEventSummaryRequest request, CancellationToken token = default)
    {
        var result = await _reportsRepository.GetEventSummaryAsync(request.EventId, request.UserId, token);

        return new GetEventSummaryResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<EventSummaryItem>>(result) };
    }
}