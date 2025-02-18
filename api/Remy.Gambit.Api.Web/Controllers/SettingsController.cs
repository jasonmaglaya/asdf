using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Handlers.AppSettings.Query.Dto;
using Remy.Gambit.Api.Handlers.Features.Query.Dto;
using Remy.Gambit.Core.Cqs;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class SettingsController(IQueryHandler<GetUserFeaturesRequest, GetUserFeaturesResult> getUserFeaturesHandler,
    IQueryHandler<GetAppSettingsRequest, GetAppSettingsResult> getAppSettingsHandler
    ) : ControllerBase
{
    private readonly IQueryHandler<GetUserFeaturesRequest, GetUserFeaturesResult> _getUserFeaturesHandler = getUserFeaturesHandler;
    private readonly IQueryHandler<GetAppSettingsRequest, GetAppSettingsResult> _getAppSettingsHandler = getAppSettingsHandler;

    [HttpGet("features/me")]
    public async Task<ActionResult<GetUserFeaturesResult>> GetUserFeatures(CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var role = identity?.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(role))
        {
            return Unauthorized();
        }

        var request = new GetUserFeaturesRequest
        {
            Role = role
        };

        var result = await _getUserFeaturesHandler.HandleAsync(request, token);

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

    [AllowAnonymous]
    [HttpGet("app-settings")]
    public async Task<ActionResult<GetAppSettingsResult>> GetAppSettings(CancellationToken token)
    {
        var request = new GetAppSettingsRequest();
        var result = await _getAppSettingsHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return NotFound();
        }

        Thread.Sleep(2000);

        return Ok(result);
    }
}
