using Microsoft.EntityFrameworkCore;
using blogging_api.Models;

namespace blogging_api.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options): base(options)
    {

    }

    public DbSet<Blog> BlogPosts => Set<Blog>();
    public DbSet<BlogTags> BlogTags => Set<BlogTags>();
}