using basic_auth.Auth;
using basic_auth.StudentService;
using Microsoft.EntityFrameworkCore;

public interface IDb
{
    DbSet<User> Users { get; }
    DbSet<Session> Sessions { get; }
    DbSet<Student> Students { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}