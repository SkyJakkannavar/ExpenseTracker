using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
