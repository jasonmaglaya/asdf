using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Query
{
    public class SearchUserHandler : IQueryHandler<SearchUserRequest, SearchUserResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IFeaturesRepository _featuresRepository;
        private readonly IValidator<SearchUserRequest> _validator;
        private readonly IMapper _mapper;

        public SearchUserHandler(IValidator<SearchUserRequest> validator, IFeaturesRepository featuresRepository, IUsersRepository usersRepository, IMapper mapper)
        {
            _mapper = mapper;
            _validator = validator;
            _usersRepository = usersRepository;
            _featuresRepository = featuresRepository;
        }

        public async ValueTask<SearchUserResult> HandleAsync(SearchUserRequest request, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return new SearchUserResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var requestor = await _usersRepository.GetUserByIdAsync(request.Requestor, token);

            var features = await _featuresRepository.GetFeaturesByRoleAsync(requestor.Role, token);

            Guid? requestorId = request.Requestor;

            if(features.Contains(Constants.Features.ListAllUsers))
            {
                requestorId = null;
            }

            var result = await _usersRepository.SearchUserAsync(request.Keyword, request.IsAgent, requestorId, request.PageNumber, request.PageSize, token);

            return new SearchUserResult { IsSuccessful = true, Result = _mapper.Map<PaginatedList<User>>(result) };
        }
    }
}
