using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Data.Features;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.ActionFilters;

public class FeatureFilterAttribute : ActionFilterAttribute
{
    private readonly string[] _features;

    public FeatureFilterAttribute()
    {
        _features = [];
    }

    public FeatureFilterAttribute(params string[] features)
    {
        _features = features;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var identity = context.HttpContext.User.Identity as ClaimsIdentity;
        var role = identity?.FindFirst(ClaimTypes.Role)?.Value;

        if (role is null)
        {
            context.Result = new BadRequestObjectResult("Role is null");
            return;
        }

        if (_features.Length == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var isAuthorized = AuthorizationHelper.IsAuthorized(role, _features, context.HttpContext);       

        if (!isAuthorized)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
