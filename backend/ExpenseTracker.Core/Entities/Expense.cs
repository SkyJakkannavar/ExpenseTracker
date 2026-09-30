namespace ExpenseTracker.Core.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public required string Category { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
