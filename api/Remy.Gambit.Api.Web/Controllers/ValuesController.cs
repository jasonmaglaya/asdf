using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Api.Handlers.Roles.Query.Dto;

namespace Remy.Gambit.Api.Web.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ValuesController(IQueryHandler<GetRolesRequest, GetRolesResult> getRolesHandler) : ControllerBase
{
    private readonly IQueryHandler<GetRolesRequest, GetRolesResult> _getRolesHandler = getRolesHandler;

    [HttpGet("roles")]
    public async Task<ActionResult<GetRolesResult>> GetRoles([FromQuery] GetRolesRequest request, CancellationToken token)
    {
        var result = await _getRolesHandler.HandleAsync(request, token);

        if (!result.IsSuccessful)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
