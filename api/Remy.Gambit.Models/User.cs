namespace Remy.Gambit.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ContactNumber { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefeshTokenExpiry { get; set; }
    public bool IsActive { get; set; }
    public bool IsBettingLocked { get; set; }
    public decimal Credits { get; set; }
    public required string Role { get; set; }
    public string? ReferralCode { get; set; }
    public Guid? Upline { get; set; }
    public float? Commission { get; set; }
    public string? AgentCode { get; set; }
    public User? UplineUser { get; set; }
    public Role? UserRole { get; set; }
}