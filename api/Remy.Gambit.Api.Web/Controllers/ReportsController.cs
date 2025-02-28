using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController(
        IQueryHandler<GetEventsReportRequest, GetEventsReportResult> getEventsReportHandler
        ) : ControllerBase        
    {
        private readonly IQueryHandler<GetEventsReportRequest, GetEventsReportResult> _getEventsReportHandler = getEventsReportHandler;

        [HttpGet("events")]
        public async Task<ActionResult<GetEventsReportResult>> GetEventsReportAsync(CancellationToken token)
        {
            return Ok(await _getEventsReportHandler.HandleAsync(new GetEventsReportRequest(), token));
        }
    }
}
