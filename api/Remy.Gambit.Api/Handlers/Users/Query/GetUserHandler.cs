using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Query
{
    public class GetUserHandler : IQueryHandler<GetUserRequest, GetUserResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IFeaturesRepository _featuresRepository;
        private readonly IValidator<GetUserRequest> _validator;
        private readonly IMapper _mapper;

        public GetUserHandler(IValidator<GetUserRequest> validator, IFeaturesRepository featuresRepository, IUsersRepository usersRepository, IMapper mapper)
        {
            _mapper = mapper;
            _validator = validator;
            _usersRepository = usersRepository;
            _featuresRepository = featuresRepository;
        }

        public async ValueTask<GetUserResult> HandleAsync(GetUserRequest request, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return new GetUserResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var user = await _usersRepository.GetUserByIdAsync(request.UserId, token);

            if(request.UserId != request.Requestor)
            {
                var requestor = await _usersRepository.GetUserByIdAsync(request.Requestor, token);
                var features = await _featuresRepository.GetFeaturesByRoleAsync(requestor.Role, token);

                if (user.Upline != requestor.Id && !features.Contains(Constants.Features.ListAllUsers))
                {
                    return new GetUserResult { IsSuccessful = false };
                }

                if (user.Upline is not null)
                {
                    var upline = await _usersRepository.GetUserByIdAsync(user.Upline.Value, token);
                    user.UplineUser = upline;
                }
            }

            var userInfo = _mapper.Map<User>(user);

            return new GetUserResult { IsSuccessful = true, Result = userInfo};
        }
    }
}
