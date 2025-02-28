using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Reports;

namespace Remy.Gambit.Api.Handlers.Reports.Query;

public class GetEventsReportHandler(IReportsRepository reportsRepository, IMapper mapper) : IQueryHandler<GetEventsReportRequest, GetEventsReportResult>
{
    private readonly IReportsRepository _reportsRepository = reportsRepository;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetEventsReportResult> HandleAsync(GetEventsReportRequest request, CancellationToken token = default)
    {
        var report = await _reportsRepository.GetEventsReportAsync(token);

        return new GetEventsReportResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<EventReportItem>>(report) };
    }
}
