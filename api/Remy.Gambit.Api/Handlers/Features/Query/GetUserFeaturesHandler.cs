using Remy.Gambit.Api.Handlers.Features.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;

namespace Remy.Gambit.Api.Handlers.Features.Query
{
    public class GetUserFeaturesHandler : IQueryHandler<GetUserFeaturesRequest, GetUserFeaturesResult>
    {
        private readonly IFeaturesRepository _featuresRepository;
        public GetUserFeaturesHandler(IFeaturesRepository featuresRepository)
        {
            _featuresRepository = featuresRepository;
        }

        public async ValueTask<GetUserFeaturesResult> HandleAsync(GetUserFeaturesRequest request, CancellationToken token = default)
        {
            var result = await _featuresRepository.GetFeaturesByRoleAsync(request.Role!, token);

            return new GetUserFeaturesResult { IsSuccessful = true, Result = result };
        }
    }
}
