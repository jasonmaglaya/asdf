using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Remy.Gambit.Data.Features;

namespace Remy.Gambit.Api.Helpers;

public class AuthorizationHelper
{
    public static bool IsAuthorized(string role, string[] features, HttpContext context)
    {
        var svc = context.RequestServices;
        var featureRepository = svc.GetService<IFeaturesRepository>() ?? throw new InvalidOperationException("Could not resolve IFeaturesRepository from RestrictFeatureAttribute");

        var feats = featureRepository.GetFeaturesByRoleAsync(role, context.RequestAborted).Result;

        bool isAuthorized = false;
        foreach (string f in features)
        {
            if (feats.Contains(f))
            {
                isAuthorized |= true;
                break;
            }
        }

        return isAuthorized;
    }
}
