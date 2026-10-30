using ExpenseTrackerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Services.Expenses;

public class ExpenseService(IDb _db)
{
    public async Task<CreateExpenseResult> CreateAsync(CreateExpenseRequest request)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .ToListAsync();

        var category = await _db.Categories.FirstOrDefaultAsync(c => 
            string.Equals(c.Name, request.Name, StringComparison.OrdinalIgnoreCase));
        
        if (category is null)
        {
            category = new Category
            {
                Name = request.CategoryName
            };

            _db.Categories.Add(category);
        }

        var newExpense = new Expense
        {
            Name = request.Name,
            Amount = request.Amount,
            Category = category
        };

        _db.Expenses.Add(newExpense);
        await _db.SaveChangesAsync();

        return new CreateExpenseResult
        (
            newExpense.Name,
            newExpense.Amount,
            newExpense.Category.Name,
            newExpense.CreatedAt
        );
    }

    // public async Task<GetExpenseResult> GetAsync()
    // {
        
    // }
}