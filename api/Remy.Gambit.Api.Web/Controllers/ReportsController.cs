using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Reports.Query.Dto;
using Remy.Gambit.Api.Web.ActionFilters;
using Remy.Gambit.Core.Cqs;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController(
        IQueryHandler<GetEventsReportRequest, GetEventsReportResult> getEventsReportHandler,
        IQueryHandler<GetEventSummaryRequest, GetEventSummaryResult> getEventSummaryHandler,
        IQueryHandler<GetPlayerEventSummaryRequest, GetPlayerEventSummaryResult> getPlayerEventSummaryHandler
        ) : ControllerBase        
    {
        private readonly IQueryHandler<GetEventsReportRequest, GetEventsReportResult> _getEventsReportHandler = getEventsReportHandler;
        private readonly IQueryHandler<GetEventSummaryRequest, GetEventSummaryResult> _getEventSummaryHandler = getEventSummaryHandler;
        private readonly IQueryHandler<GetPlayerEventSummaryRequest, GetPlayerEventSummaryResult> _getPlayerEventSummaryHandler = getPlayerEventSummaryHandler;

        [FeatureFilter(Features.Reports)]
        [HttpGet("events")]
        public async Task<ActionResult<GetEventsReportResult>> GetEventsReportAsync(CancellationToken token)
        {
            return Ok(await _getEventsReportHandler.HandleAsync(new GetEventsReportRequest(), token));
        }

        [FeatureFilter(Features.Reports)]
        [HttpGet("events/{id}")]
        public async Task<ActionResult<GetEventSummaryResult>> GetEventSummary([FromRoute] Guid id, CancellationToken token)
        {
            var request = new GetEventSummaryRequest
            {
                EventId = id
            };

            var result = await _getEventSummaryHandler.HandleAsync(request, token);

            if (result.ValidationResults.Any())
            {
                return BadRequest(result);
            }

            if (!result.IsSuccessful)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("players/events/{id}")]
        public async Task<ActionResult<GetPlayerEventSummaryResult>> GetPlayerEventSummary([FromRoute] Guid id, CancellationToken token)
        {
            var request = new GetPlayerEventSummaryRequest
            {
                EventId = id
            };

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
            {
                return Unauthorized();
            }

            request.UserId = userId;

            var result = await _getPlayerEventSummaryHandler.HandleAsync(request, token);

            if (result.ValidationResults.Any())
            {
                return BadRequest(result);
            }

            if (!result.IsSuccessful)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
