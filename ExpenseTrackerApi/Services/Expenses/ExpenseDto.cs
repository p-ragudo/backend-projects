namespace ExpenseTrackerApi.Services.Expenses;

public record CreateExpenseRequest(
    string Name, 
    decimal Amount
);

public record CreateExpenseResult(
    string Name, 
    decimal Amount, 
    DateTime CreatedAt
);


public record GetExpenseRequest(
    string[]? Names,
    decimal[]? Amounts
);

public record GetExpenseResult(
    List<Expense> Expenses
);