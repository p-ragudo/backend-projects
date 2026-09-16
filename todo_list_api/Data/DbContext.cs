using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using todo_list_api.TodoService;

namespace todo_list_api;

public class DbContext: IdentityDbContext<IdentityUser>
{
    public DbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("todo_list_api");
    }
}