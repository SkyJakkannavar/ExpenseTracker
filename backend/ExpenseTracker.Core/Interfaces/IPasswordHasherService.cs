namespace ExpenseTracker.Core.Interfaces;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
