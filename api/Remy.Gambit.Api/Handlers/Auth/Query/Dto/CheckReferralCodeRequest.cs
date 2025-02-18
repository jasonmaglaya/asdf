using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Query.Dto
{
    public class CheckReferralCodeRequest : IQuery
    {
        public required string ReferralCode { get; set; }
    }
}
