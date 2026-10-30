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


public record GetExpenseRequest(
    string[]? Names,
    string[]? Categories,
    decimal[]? Amounts
);

public record GetExpenseResult(
    List<Expense> Expenses
);