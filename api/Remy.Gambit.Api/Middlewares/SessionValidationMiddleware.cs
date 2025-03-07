using Microsoft.AspNetCore.Http;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Core.Caching;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Remy.Gambit.Api.Middlewares;

public class SessionValidationMiddleware(RequestDelegate next, ICacheService cache)
{
    private readonly RequestDelegate _next = next;
    
    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var sessionId = jwtToken?.Claims.FirstOrDefault(c => c.Type == Config.SessionID)?.Value;

            if (userId != null && sessionId != null)
            {
                if(cache.TryGetValue<string>($"{Config.SessionID}:{userId}", out var session))
                {
                    if (session != sessionId)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
