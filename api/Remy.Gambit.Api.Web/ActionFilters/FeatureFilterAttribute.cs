using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Remy.Gambit.Data.Features;
using System.Security.Claims;

namespace Remy.Gambit.Api.Web.ActionFilters;

public class FeatureFilterAttribute : ActionFilterAttribute
{
    private readonly string[] _features;

    public FeatureFilterAttribute()
    {
        _features = Array.Empty<string>();
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

        if (!_features.Any())
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var svc = context.HttpContext.RequestServices;
        var featureRepository = svc.GetService<IFeaturesRepository>() ?? throw new InvalidOperationException("Could not resolve IFeaturesReporsitory from RestrictFeatureAttribute");

        var features = featureRepository.GetFeaturesByRoleAsync(role, context.HttpContext.RequestAborted).Result;

        bool isAutorized = false;
        foreach (string f in _features)
        {
            if (features.Contains(f))
            {
                isAutorized |= true;
                break;
            }
        }

        if (!isAutorized)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
