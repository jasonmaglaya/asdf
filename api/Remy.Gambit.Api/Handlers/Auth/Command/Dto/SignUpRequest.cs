using Remy.Gambit.Core.Cqs;

namespace Remy.Gambit.Api.Handlers.Auth.Command.Dto
{
    public class SignUpRequest : ICommand
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string ContactNumber { get; set; }
        public string? ReferralCode { get; set; }
    }    
}
