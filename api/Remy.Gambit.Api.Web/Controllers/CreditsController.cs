using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Handlers.Credits.Command.Dto;
using Remy.Gambit.Api.Handlers.Credits.Request.Dto;
using Remy.Gambit.Core.Cqs;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CreditsController(
        IQueryHandler<GetUserBalanceRequest, GetUserBalanceResult> getUserBalanceHandler,
        ICommandHandler<CashInRequest, CashInResult> cashInHandler, 
        ICommandHandler<CashOutRequest, CashOutResult> cashOutHandler,
        IQueryHandler<GetCreditHistoryRequest, GetCreditHistoryResult> getCreditHistoryHandler
    ) : ControllerBase
    {
        private readonly IQueryHandler<GetUserBalanceRequest, GetUserBalanceResult> _getUserBalanceHandler = getUserBalanceHandler;
        private readonly ICommandHandler<CashInRequest, CashInResult> _cashInHandler = cashInHandler;
        private readonly ICommandHandler<CashOutRequest, CashOutResult> _cashOutHandler = cashOutHandler;
        private readonly IQueryHandler<GetCreditHistoryRequest, GetCreditHistoryResult> _getCreditHistoryHandler = getCreditHistoryHandler;

        [HttpGet]
        public async Task<ActionResult<GetUserBalanceResult>> GetUserBalance([FromQuery] GetUserBalanceRequest request, CancellationToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
            {
                return Unauthorized();
            }

            request.UserId = userId;

            var result = await _getUserBalanceHandler.HandleAsync(request, token);

            if (result.ValidationResults.Any())
            {
                return BadRequest(result);
            }

            if (!result.IsSuccessful)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<GetCreditHistoryResult>> GetHistory([FromQuery] GetCreditHistoryRequest request, CancellationToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
            {
                return Unauthorized();
            }

            request.UserId = userId;

            var result = await _getCreditHistoryHandler.HandleAsync(request, token);

            if (result.ValidationResults.Any())
            {
                return BadRequest(result);
            }

            if (!result.IsSuccessful)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("cash-in")]
        public async Task<ActionResult<CashInResult>> CashIn([FromBody] CashInRequest request, CancellationToken token)
        {
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

            var result = await _cashInHandler.HandleAsync(request, token);

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

        [HttpPost("cash-out")]
        public async Task<ActionResult<CashOutResult>> CashIn([FromBody] CashOutRequest request, CancellationToken token)
        {
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

            var result = await _cashOutHandler.HandleAsync(request, token);

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
    }
}
