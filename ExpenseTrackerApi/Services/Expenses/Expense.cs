namespace ExpenseTrackerApi.Services.Expenses;

public class Expense
{
    public Guid Id { get; set; }
    public string Name { get;set; } = string.Empty;
    public decimal Amount { get; set; }
    public required Category Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}