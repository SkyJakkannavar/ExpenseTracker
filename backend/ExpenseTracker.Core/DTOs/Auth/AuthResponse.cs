namespace ExpenseTracker.Core.DTOs.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string Email { get; set; }
}
