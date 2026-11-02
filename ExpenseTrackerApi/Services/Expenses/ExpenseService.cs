using ExpenseTrackerApi.Common;
using ExpenseTrackerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Services.Expenses;

public class ExpenseService(IDb _db)
{
    public async Task<Result<CreateExpenseResult>> CreateAsync(CreateExpenseRequest request)
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

        var result = new CreateExpenseResult
        (
            newExpense.Name,
            newExpense.Amount,
            newExpense.Category.Name,
            newExpense.CreatedAt
        );

        return Result<CreateExpenseResult>.Success(result);
    }

    public async Task<Result<GetExpenseResult>> GetAsync()
    {
        var expenseResult = new GetExpenseResult(
            await _db.Expenses.ToListAsync()
        );

        return Result<GetExpenseResult>.Success(expenseResult);
    }

    public async Task<Result<UpdateExpenseResult>> UpdateAsync(UpdateExpenseRequest request)
    {
        if (!Guid.TryParse(request.Guid, out var requestGuid))
        {
            return Result<UpdateExpenseResult>.Failure(ErrorType.InvalidFormat);
        }
        
        var existingExpenses = await _db.Expenses
            .FirstOrDefaultAsync(e => e.Id == requestGuid);
        
        if (existingExpenses is null)
        {
            return Result<UpdateExpenseResult>.Failure(ErrorType.NotFound);
        }
        
        existingExpenses.Name = request.Name ?? existingExpenses.Name;
        existingExpenses.Amount = request.Amount ?? existingExpenses.Amount;
        existingExpenses.Category.Name = request.Category ?? existingExpenses.Category.Name;

        await _db.SaveChangesAsync();

        return Result<UpdateExpenseResult>.Success(new UpdateExpenseResult(existingExpenses));
    }

    public async Task<Result<DeleteExpenseResult>> DeleteAsync(string expenseGuid)
    {
        if (!Guid.TryParse(expenseGuid, out var expenseGuidConverted))
        {
            return Result<DeleteExpenseResult>.Failure(ErrorType.InvalidFormat);
        }

        await _db.Expenses
            .Where(e => e.Id == expenseGuidConverted)
            .ExecuteDeleteAsync();
        
        await _db.SaveChangesAsync();

        return Result<DeleteExpenseResult>.Success();
    }
}