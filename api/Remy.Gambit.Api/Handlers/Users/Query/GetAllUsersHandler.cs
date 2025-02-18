using AutoMapper;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Users;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Users.Query
{
    public class GetAllUsersHandler : IQueryHandler<GetAllUsersRequest, GetAllUsersResult>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(IUsersRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = userRepository;
        }

        public async ValueTask<GetAllUsersResult> HandleAsync(GetAllUsersRequest request, CancellationToken token = default)
        {
            var result = await _usersRepository.GetAllUsersAsync(request.IsAgent, request.PageNumber, request.PageSize, token);

            return new GetAllUsersResult { IsSuccessful = true, Result = _mapper.Map<PaginatedList<User>, PaginatedList<Api.Dto.User>>(result) };
        }
    }
}
