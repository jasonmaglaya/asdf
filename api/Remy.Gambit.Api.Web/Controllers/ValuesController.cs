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

    [AllowAnonymous]
    [HttpGet("my-ip")]
    public ActionResult<string> MyIp(CancellationToken token)
    {
        var xForwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();        

        return Ok($"xForwardedFor : {xForwardedFor}\nremoteIpAddress:{remoteIpAddress}");
    }

    [AllowAnonymous]
    [HttpGet("test")]
    public ActionResult<string> Test(CancellationToken token)
    {
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        var allowedIps = new string[] {
            "45.192.223.30",
            "45.192.223.20",
            "45.192.223.10"
        };

        if(allowedIps.Contains(ip))
        {
            return $"Your IP Address {ip} is allowed.\n\nAllowed IPs are:\n{string.Join("\n", allowedIps)}";
        }
        else
        {
            return $"Your IP Address {ip} is NOT allowed.\n\nAllowed IPs are:\n{string.Join("\n" , allowedIps)}";
        }
    }

    //[AllowAnonymous]
    //[HttpGet("my-ip/bounce")]
    //public async Task<ActionResult<string>> BounceMyIp(CancellationToken token)
    //{
    //    var httpClient = new HttpClient();
    //    var response = await httpClient.GetAsync("https://api.ipify.org");

    //    var result = await response.Content.ReadAsStringAsync();

    //    return Ok(result);
    //}

    [AllowAnonymous]
    [HttpGet("image")]
    public IActionResult GetContainerImage()
    {
        var imageName = Environment.GetEnvironmentVariable("IMAGE_NAME") ?? "Unknown";
        return Ok(new { image = imageName });
    }
}
