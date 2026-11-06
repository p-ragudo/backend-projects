namespace ExpenseTrackerApi.Services.Expenses;

public record CreateCategoryRequest(
    string Name
);

public record CreateCategoryResult(
    string Name,
    DateTime CreatedAt
);

public record GetCategoryRequest(
    string[]? Names
);

public record GetCategoryResult(
    List<Category> Categories
);