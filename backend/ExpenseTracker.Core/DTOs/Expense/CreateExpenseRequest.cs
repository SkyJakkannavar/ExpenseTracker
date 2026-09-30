namespace ExpenseTracker.Core.DTOs.Expense;

public class CreateExpenseRequest
{
    public decimal Amount { get; set; }
    public required string Category { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}
