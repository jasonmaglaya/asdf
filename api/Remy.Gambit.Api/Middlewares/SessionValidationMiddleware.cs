using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Data.Users;
using System.IdentityModel.Tokens.Jwt;

namespace Remy.Gambit.Api.Middlewares;

public class SessionValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var sessionId = jwtToken?.Claims.FirstOrDefault(c => c.Type == Config.SessionID)?.Value;

            if (sessionId != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();

                var user = await userRepository.GetUserByRefreshTokenAsync(sessionId, default);
                if (user is null)
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
        }

        await _next(context);
    }
}
