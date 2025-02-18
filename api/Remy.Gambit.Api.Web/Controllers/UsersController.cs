using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Users.Command.Dto;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Api.Web.ActionFilters;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UsersController(
    IQueryHandler<GetUserRequest, GetUserResult> getUserInfoHandler,
    IQueryHandler<GetDownLinesRequest, GetDownLinesResult> getDownlinesHandler,
    ICommandHandler<UpdateUserStatusRequest, UpdateUserStatusResult> changeUserStatusHandler,
    ICommandHandler<UpdateUserRoleRequest, UpdateUserRoleResult> updateUserRoleHandler,
    ICommandHandler<UpdateAgencyRequest, UpdateAgencyResult> updateAgencyHandler,
    IQueryHandler<GetAllUsersRequest, GetAllUsersResult> getAllUsersHandler,
    ICommandHandler<TransferCreditsRequest, TransferCreditsResult> transferCreditsHandler,
    IQueryHandler<SearchUserRequest, SearchUserResult> searchUserHandler
        ) : ControllerBase
{
    private readonly IQueryHandler<GetUserRequest, GetUserResult> _getUserInfoHandler = getUserInfoHandler;
    private readonly IQueryHandler<GetDownLinesRequest, GetDownLinesResult> _getDownLinesHandler = getDownlinesHandler;
    private readonly ICommandHandler<UpdateUserStatusRequest, UpdateUserStatusResult> _changeUserStatusHandler = changeUserStatusHandler;
    private readonly ICommandHandler<UpdateUserRoleRequest, UpdateUserRoleResult> _updateUserRoleHandler = updateUserRoleHandler;
    private readonly ICommandHandler<UpdateAgencyRequest, UpdateAgencyResult> _updateAgencyHandler = updateAgencyHandler;
    private readonly IQueryHandler<GetAllUsersRequest, GetAllUsersResult> _getAllUsersHandler = getAllUsersHandler;
    private readonly ICommandHandler<TransferCreditsRequest, TransferCreditsResult> _transferCreditsHandler = transferCreditsHandler;
    private readonly IQueryHandler<SearchUserRequest, SearchUserResult> _searchUserHandler = searchUserHandler;

    [FeatureFilter(Features.ListAllUsers)]
    [HttpGet]
    public async Task<ActionResult<GetAllUsersResult>> GetAllUsers([FromQuery] GetAllUsersRequest request, CancellationToken token)
    {
        var result = await _getAllUsersHandler.HandleAsync(request, token);

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

    [HttpGet("me")]
    public async Task<ActionResult<GetUserResult>> Me(CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        var request = new GetUserRequest { UserId = userId, Requestor = userId };

        var result = await _getUserInfoHandler.HandleAsync(request, token);

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

    [HttpGet("me/down-lines")]
    public async Task<ActionResult<GetDownLinesResult>> GetDownLines(CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        var request = new GetDownLinesRequest { UserId = userId };

        var result = await _getDownLinesHandler.HandleAsync(request, token);

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
    public async Task<ActionResult<GetUserResult>> GetUser([FromRoute] Guid id, CancellationToken token)
    {
        var request = new GetUserRequest { UserId = id };

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.Requestor = userId;

        var result = await _getUserInfoHandler.HandleAsync(request, token);

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

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<UpdateUserStatusResult>> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateUserStatusRequest request, CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.Requestor = userId;
        request.UserId = id;

        var result = await _changeUserStatusHandler.HandleAsync(request, token);

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

    [FeatureFilter(Features.UpdateRole)]
    [HttpPatch("{id}/role")]
    public async Task<ActionResult<UpdateUserRoleResult>> UpdateRole([FromRoute] Guid id, [FromBody] UpdateUserRoleRequest request, CancellationToken token)
    {
        request.UserId = id;

        var result = await _updateUserRoleHandler.HandleAsync(request, token);

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

    [HttpPatch("{id}/agency")]
    public async Task<ActionResult<UpdateAgencyResult>> UpdateAgency([FromRoute] Guid id, [FromBody] UpdateAgencyRequest request, CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.Requestor = userId;
        request.UserId = id;

        var result = await _updateAgencyHandler.HandleAsync(request, token);

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

    [HttpPost("{id}/credits")]
    public async Task<ActionResult<TransferCreditsResult>> TransferCredits([FromRoute] Guid id, [FromBody] TransferCreditsRequest request, CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.Requestor = userId;
        request.UserId = id;

        var result = await _transferCreditsHandler.HandleAsync(request, token);

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

    [HttpGet("search")]
    public async Task<ActionResult<SearchUserResult>> SearchUser([FromQuery] SearchUserRequest request, CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        request.Requestor = userId;

        var result = await _searchUserHandler.HandleAsync(request, token);

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
