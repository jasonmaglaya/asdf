namespace Remy.Gambit.Api.Dto
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public string? Role { get; set; }
        public decimal Credits { get; set; }
        public string? ReferralCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsBettingLocked { get; set; }
        public float? Commission { get; set; }
        public User? UplineUser { get; set; }
        public Role? UserRole { get; set; }
    }
}
