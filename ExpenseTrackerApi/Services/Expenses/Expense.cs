namespace ExpenseTrackerApi.Services.Expenses;

public class Expense
{
    public Guid Id;
    public string Name { get;set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}