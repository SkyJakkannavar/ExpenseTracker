using ExpenseTracker.Core.Entities;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseRepository
{
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(Guid userId);
    Task UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(Guid id, Guid userId);
}
