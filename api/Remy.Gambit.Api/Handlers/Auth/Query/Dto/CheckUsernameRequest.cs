using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Query.Dto
{
    public class CheckUsernameRequest : IQuery
    {
        public required string Username { get; set; }
    }
}
