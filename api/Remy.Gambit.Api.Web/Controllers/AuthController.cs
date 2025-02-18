using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Api.Handlers.Auth.Query.Dto;
using Remy.Gambit.Core.Cqs;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(
    ICommandHandler<SignUpRequest, SignUpResult> signupHandler,
    IQueryHandler<CheckUsernameRequest, CheckUsernameResult> checkUsernameHandler,
    IQueryHandler<CheckReferralCodeRequest, CheckReferralCodeResult> checkReferralCodeHandler,
    ICommandHandler<LoginRequest, LoginResult> loginHandler,
    ICommandHandler<RefreshTokenRequest, RefreshTokenResult> refreshTokenHandler,
    ICommandHandler<LogoutRequest, LogoutResult> logoutHandler,
    ICommandHandler<AdHocLoginRequest, AdHocLoginResult> adHocLoginHandler
        ) : ControllerBase
{
    private readonly ICommandHandler<SignUpRequest, SignUpResult> _signupHandler = signupHandler;
    private readonly IQueryHandler<CheckUsernameRequest, CheckUsernameResult> _checkUsernameHandler = checkUsernameHandler;
    private readonly IQueryHandler<CheckReferralCodeRequest, CheckReferralCodeResult> _checkReferralCodeHandler = checkReferralCodeHandler;
    private readonly ICommandHandler<LoginRequest, LoginResult> _loginHandler = loginHandler;
    private readonly ICommandHandler<RefreshTokenRequest, RefreshTokenResult> _refreshTokenHandler = refreshTokenHandler;
    private readonly ICommandHandler<LogoutRequest, LogoutResult> _logoutHandler = logoutHandler;
    private readonly ICommandHandler<AdHocLoginRequest, AdHocLoginResult> _adHocLoginHandler = adHocLoginHandler;

    [HttpPost("signup")]
    public async Task<ActionResult<SignUpResult>> Signup([FromBody] SignUpRequest request, CancellationToken token)
    {
        var result = await _signupHandler.HandleAsync(request, token);

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

    [HttpGet("username/check")]
    public async Task<ActionResult<CheckUsernameResult>> CheckUsername([FromQuery] CheckUsernameRequest request, CancellationToken token)
    {
        var result = await _checkUsernameHandler.HandleAsync(request, token);

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

    [HttpGet("referral-code/check")]
    public async Task<ActionResult<CheckReferralCodeResult>> CheckReferralCode([FromQuery] CheckReferralCodeRequest request, CancellationToken token)
    {
        var result = await _checkReferralCodeHandler.HandleAsync(request, token);

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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request, CancellationToken token)
    {
        var result = await _loginHandler.HandleAsync(request, token);

        if (result.ValidationResults.Any())
        {
            return BadRequest(result);
        }

        if (!result.IsSuccessful)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("player-login")]
    public async Task<ActionResult<AdHocLoginResult>> AdHocLogin([FromBody] AdHocLoginRequest request, CancellationToken token)
    {
        if (Request.Headers.TryGetValue("x-client-id", out var clientId))
        {
            request.ClientId = clientId;
        }
        //else
        //{
        //    return Unauthorized("Header x-client-id not found.");
        //}

        if (Request.Headers.TryGetValue("x-client-secret", out var clientSecret))
        {
            request.ClientSecret = clientSecret;
        }
        //else
        //{
        //    return Unauthorized("Header x-client-secret not found.");
        //}

        var clientIp = Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        request.IpAddress = clientIp;

        var result = await _adHocLoginHandler.HandleAsync(request, token);

        //if (result.Status! != "success")
        //{
        //    return Unauthorized();
        //}

        return Ok(result);
    }

    [HttpPost("access-token/refresh")]
    public async Task<ActionResult<RefreshTokenResult>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken token)
    {
        var result = await _refreshTokenHandler.HandleAsync(request, token);

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

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<LogoutResult>> Logout(CancellationToken token)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
        {
            return Unauthorized();
        }

        var request = new LogoutRequest { UserId = userId };

        var result = await _logoutHandler.HandleAsync(request, token);

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
