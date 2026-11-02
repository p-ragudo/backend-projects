namespace ExpenseTrackerApi.Services.Expenses;

public record CreateExpenseRequest(
    string Name, 
    decimal Amount,
    string CategoryName
);

public record CreateExpenseResult(
    string Name, 
    decimal Amount, 
    string CategoryName,
    DateTime CreatedAt
);

public record GetExpenseResult(
    List<Expense> Expenses
);

public record UpdateExpenseRequest(
    string Guid,
    string? Name,
    decimal? Amount,
    string? Category
);

public record UpdateExpenseResult(
    Expense Expense
);

public record DeleteExpenseResult(
    Expense Expense
);