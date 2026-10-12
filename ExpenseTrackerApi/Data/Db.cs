using ExpenseTrackerApi.Services.Auth;
using ExpenseTrackerApi.Services.Expenses;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Data;

public class Db : DbContext, IDb
{
    public Db(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("ExpenseTrackerApi");
    }
}

