using ExpenseTrackerApi.Data;

namespace ExpenseTrackerApi.Services.Expenses;

public class ExpenseService(IDb _db)
{
    public async Task<CreateExpenseResult> CreateAsync(CreateExpenseRequest request)
    {
        var newExpense = new Expense
        {
            Name = request.Name,
            Amount = request.Amount
        };

        _db.Expenses.Add(newExpense);
        await _db.SaveChangesAsync();

        return new CreateExpenseResult
        (
            newExpense.Name,
            newExpense.Amount,
            newExpense.CreatedAt
        );
    }

    public async Task<GetExpenseResult> GetAsync()
    {
        
    }
}