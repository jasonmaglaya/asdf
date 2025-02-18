using AutoMapper;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Roles.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Roles;

namespace Remy.Gambit.Api.Handlers.Roles.Query
{
    public class GetRolesHandler : IQueryHandler<GetRolesRequest, GetRolesResult>
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IMapper _mapper;

        public GetRolesHandler(IRolesRepository rolesRepository, IMapper mapper)
        {
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        public async ValueTask<GetRolesResult> HandleAsync(GetRolesRequest request, CancellationToken token = default)
        {
            var roles = await _rolesRepository.GetRolesAsync(token);

            return new GetRolesResult { IsSuccessful = true, Result = _mapper.Map<IEnumerable<Role>>(roles) };
        }
    }
}
