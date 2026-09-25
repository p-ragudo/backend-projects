using Microsoft.EntityFrameworkCore;
using basic_auth.StudentService;
using basic_auth.Auth;

namespace basic_auth.Data;

public class Db : DbContext, IDb
{
    public Db(DbContextOptions options) : base(options)
    {  
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("basic_auth");
    }
}