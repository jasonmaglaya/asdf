using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Events.Command.Dto;
using Remy.Gambit.Api.Handlers.Events.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Api.Web.ActionFilters;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;

namespace Remy.Gambit.Api.Web.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class EventsController(
    ICommandHandler<AddEventRequest, AddEventResult> addEventHandler,
    ICommandHandler<UpdateEventRequest, UpdateEventResult> updateEventHandler,
    ICommandHandler<UpdateEventStatusRequest, UpdateEventStatusResult> updateEventStatusHandler,
    IQueryHandler<GetEventsRequest, GetEventsResult> getEventsHandler,
    IQueryHandler<GetEventRequest, GetEventResult> getEventHandler,
    IQueryHandler<GetCurrentMatchRequest, GetCurrentMatchResult> getCurrentMatchHandler,
    ICommandHandler<NextMatchRequest, NextMatchResult> nextMatchHandler,
    IQueryHandler<GetEventWinnersRequest, GetEventWinnersResult> getEventWinnersHandler,
    IQueryHandler<GetMatchesRequest, GetMatchesResult> getMatchesHandler
    ) : ControllerBase
{
    private readonly ICommandHandler<AddEventRequest, AddEventResult> _addEventHandler = addEventHandler;
    private readonly ICommandHandler<UpdateEventRequest, UpdateEventResult> _updateEventHandler = updateEventHandler;
    private readonly ICommandHandler<UpdateEventStatusRequest, UpdateEventStatusResult> _updateEventStatusHandler = updateEventStatusHandler;
    private readonly IQueryHandler<GetEventsRequest, GetEventsResult> _getEventsHandler = getEventsHandler;
    private readonly IQueryHandler<GetEventRequest, GetEventResult> _getEventHandler = getEventHandler;
    private readonly IQueryHandler<GetCurrentMatchRequest, GetCurrentMatchResult> _getCurrentMatchHandler = getCurrentMatchHandler;
    private readonly ICommandHandler<NextMatchRequest, NextMatchResult> _nextMatchHandler = nextMatchHandler;
    private readonly IQueryHandler<GetEventWinnersRequest, GetEventWinnersResult> _getEventWinnersHandler = getEventWinnersHandler;
    private readonly IQueryHandler<GetMatchesRequest, GetMatchesResult> _getMatchesHandler = getMatchesHandler;

    [FeatureFilter(Features.MaintainEvents)]
    [HttpPost]
    public async Task<ActionResult<AddEventResult>> AddEvent([FromBody] AddEventRequest request, CancellationToken token)
    {
        var result = await _addEventHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [FeatureFilter(Features.MaintainEvents)]
    [HttpPut]
    public async Task<ActionResult<UpdateEventResult>> UpdateEvent([FromBody] UpdateEventRequest request, CancellationToken token)
    {
        var result = await _updateEventHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [FeatureFilter(Features.MaintainEvents)]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<AddEventResult>> UpdateEventStatus([FromRoute] Guid id, [FromBody] UpdateEventStatusRequest request, CancellationToken token)
    {
        request.EventId = id;

        var result = await _updateEventStatusHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [FeatureFilter(Features.MaintainEvents)]
    [HttpGet("all")]
    public async Task<ActionResult<GetEventsResult>> GetAllEvents([FromQuery] GetEventsRequest request, CancellationToken token)
    {
        var result = await _getEventsHandler.HandleAsync(request, token);

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

    [HttpGet]
    public async Task<ActionResult<GetEventsResult>> GetEvents([FromQuery] GetEventsRequest request, CancellationToken token)
    {
        request.Status = EventStatuses.Active;

        var result = await _getEventsHandler.HandleAsync(request, token);

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

    [HttpGet("{id}")]
    public async Task<ActionResult<GetEventResult>> GetEvent([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetEventRequest
        {
            Id = id
        };

        var result = await _getEventHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.MaintainEvents)]
    [HttpGet("{id}/matches")]
    public async Task<ActionResult<GetMatchesResult>> GetMatches([FromRoute] Guid id, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken token)
    {
        var request = new GetMatchesRequest
        {
            EventId = id,
            PageNumber = pageNumber ?? 1,
            PageSize = pageSize ?? 50
        };

        var result = await _getMatchesHandler.HandleAsync(request, token);

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

    [HttpGet("{id}/matches/current")]
    public async Task<ActionResult<GetCurrentMatchResult>> GetCurrentMatch([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetCurrentMatchRequest
        {
            EventId = id
        };

        var result = await _getCurrentMatchHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.ControlMatch)]
    [HttpPost("{id}/matches/next")]
    public async Task<ActionResult<NextMatchResult>> GetNextMatch([FromRoute] Guid id, CancellationToken token)
    {
        var request = new NextMatchRequest
        {
            EventId = id
        };

        var result = await _nextMatchHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpGet("{id}/winners")]
    public async Task<ActionResult<GetEventWinnersResult>> GetWinners([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetEventWinnersRequest
        {
            EventId = id
        };

        var result = await _getEventWinnersHandler.HandleAsync(request, token);

        if(result.ValidationResults.Any())
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
