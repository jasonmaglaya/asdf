using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Matches.Command.Dto;
using Remy.Gambit.Api.Handlers.Matches.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Api.Web.ActionFilters;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class MatchesController(
    IQueryHandler<GetMatchRequest, GetMatchResult> getMatchHandler,
    ICommandHandler<AddBetRequest, AddBetResult> addBetHandler,
    IQueryHandler<GetTotalBetsRequest, GetTotalBetsResult> getTotalBetsHandler,
    IQueryHandler<GetBetsRequest, GetBetsResult> getBetsHandler,
    ICommandHandler<UpdateStatusRequest, UpdateStatusResult> updateStatusHandler,
    ICommandHandler<DeclareWinnerRequest, DeclareWinnerResult> declareWinnerHandler,
    ICommandHandler<ReDeclareWinnerRequest, ReDeclareWinnerResult> reDeclareWinnerHandler,
    ICommandHandler<CancelMatchRequest, CancelMatchResult> cancelMatchHandler
    ) : ControllerBase
{
    private readonly IQueryHandler<GetMatchRequest, GetMatchResult> _getMatchHandler = getMatchHandler;
    private readonly ICommandHandler<AddBetRequest, AddBetResult> _addBetHandler = addBetHandler;
    private readonly IQueryHandler<GetTotalBetsRequest, GetTotalBetsResult> _getTotalBetsHandler = getTotalBetsHandler;
    private readonly IQueryHandler<GetBetsRequest, GetBetsResult> _getBetsHandler = getBetsHandler;
    private readonly ICommandHandler<UpdateStatusRequest, UpdateStatusResult> _updateStatusHandler = updateStatusHandler;
    private readonly ICommandHandler<DeclareWinnerRequest, DeclareWinnerResult> _declareWinnerHandler = declareWinnerHandler;
    private readonly ICommandHandler<ReDeclareWinnerRequest, ReDeclareWinnerResult> _reDeclareWinnerHandler = reDeclareWinnerHandler;
    private readonly ICommandHandler<CancelMatchRequest, CancelMatchResult> _cancelMatchHandler = cancelMatchHandler;

    [HttpGet("{id}")]
    public async Task<ActionResult<GetMatchResult>> GetMatch([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetMatchRequest
        {
            MatchId = id
        };

        var result = await _getMatchHandler.HandleAsync(request, token);

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

    [HttpPost("{id}/bets")]
    public async Task<ActionResult<AddBetResult>> AddBet([FromRoute] Guid id, [FromBody] AddBetRequest request, CancellationToken token)
    {
        request.MatchId = id;

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.UserId = userId;
        
        var clientIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        request.IpAddress = clientIp;

        var result = await _addBetHandler.HandleAsync(request, token);

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

    [HttpGet("{id}/bets/me")]
    public async Task<ActionResult<GetBetsResult>> GetBets([FromRoute] Guid id, CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return BadRequest();
        }

        var request = new GetBetsRequest
        {
            UserId = userId,
            MatchId = id
        };

        var result = await _getBetsHandler.HandleAsync(request, token);

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

    [HttpGet("{id}/bets/total")]
    public async Task<ActionResult<GetTotalBetsResult>> GetTotalBets([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetTotalBetsRequest
        {
            MatchId = id
        };

        var result = await _getTotalBetsHandler.HandleAsync(request, token);

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
    [HttpPost("{id}/status")]
    public async Task<ActionResult<UpdateStatusResult>> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateStatusRequest request, CancellationToken token)
    {
        request.MatchId = id;
        var result = await _updateStatusHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.ControlMatch)]
    [HttpPost("{id}/winner")]
    public async Task<ActionResult<DeclareWinnerResult>> DeclareWinner([FromRoute] Guid id, [FromBody] DeclareWinnerRequest request, CancellationToken token)
    {
        request.MatchId = id;

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return BadRequest();
        }

        request.UserId = userId;

        var clientIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        request.IpAddress = clientIp;

        var result = await _declareWinnerHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.ReDeclare)]
    [HttpPost("{id}/winner/re-declare")]
    public async Task<ActionResult<ReDeclareWinnerResult>> ReDeclareWinner([FromRoute] Guid id, [FromBody] ReDeclareWinnerRequest request, CancellationToken token)
    {
        request.MatchId = id;

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return BadRequest();
        }

        request.UserId = userId;

        var clientIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        request.IpAddress = clientIp;

        var result = await _reDeclareWinnerHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.ControlMatch)]
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<CancelMatchResult>> CancelMatch([FromRoute] Guid id, [FromBody] CancelMatchRequest request, CancellationToken token)
    {
        request.MatchId = id;

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return BadRequest();
        }

        request.CancelledBy = userId;

        var clientIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        request.IpAddress = clientIp;

        var result = await _cancelMatchHandler.HandleAsync(request, token);

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
}
