using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Remy.Gambit.Api.Handlers.Credits.Dto;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Api.Handlers.Users.Query;
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
        ICommandHandler<CashOutRequest, CashOutResult> cashOutHandler
    ) : ControllerBase
    {
        private readonly IQueryHandler<GetUserBalanceRequest, GetUserBalanceResult> _getUserBalanceHandler = getUserBalanceHandler;
        private readonly ICommandHandler<CashInRequest, CashInResult> _cashInHandler = cashInHandler;
        private readonly ICommandHandler<CashOutRequest, CashOutResult> _cashOutHandler = cashOutHandler;

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

        [HttpPost("cash-in")]
        public async Task<ActionResult<CashInResult>> CashIn([FromBody] CashInRequest request, CancellationToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.Name)?.Value!, out Guid userId))
            {
                return Unauthorized();
            }

            request.UserId = userId;

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
