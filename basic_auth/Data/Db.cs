using Microsoft.EntityFrameworkCore;
using basic_auth.StudentService;

namespace basic_auth.Data;

public class Db : DbContext
{
    public Db(DbContextOptions options) : base(options)
    {  
    }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("basic_auth");
    }
}