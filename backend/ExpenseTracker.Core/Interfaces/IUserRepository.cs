using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(string email, string passwordHash);
    Task<User?> GetUserByEmailAsync(string email);
}
