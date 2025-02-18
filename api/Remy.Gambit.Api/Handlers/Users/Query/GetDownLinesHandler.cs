using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Users.Query
{
    public class GetDownLinesHandler : IQueryHandler<GetDownLinesRequest, GetDownLinesResult>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IValidator<GetDownLinesRequest> _validator;
        private readonly IMapper _mapper;

        public GetDownLinesHandler(IValidator<GetDownLinesRequest> validator, IUsersRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _validator = validator;
            _userRepository = userRepository;
        }

        public async ValueTask<GetDownLinesResult> HandleAsync(GetDownLinesRequest request, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(request, token);
            if (!validationResult.IsValid)
            {
                return new GetDownLinesResult { IsSuccessful = false, Errors = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            var users = await _userRepository.GetDownLinesAsync(request.UserId, token);

            var downlines = _mapper.Map<IEnumerable<User>>(users);

            return new GetDownLinesResult { IsSuccessful = true, Result = downlines };
        }
    }
}
