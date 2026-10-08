using ExpenseTrackerApi.Services.Auth;
using ExpenseTrackerApi.Services.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Data;

public interface IDb
{
    DbSet<User> Users { get; }
    DbSet<Expense> Expenses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}