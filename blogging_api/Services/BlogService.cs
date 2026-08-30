using blogging_api.Data;
using blogging_api.Dtos;
using blogging_api.Models;
using Microsoft.EntityFrameworkCore;

namespace blogging_api.Services;

public class BlogService
{
    private readonly BlogDbContext _context;
    public BlogService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetBlogResponse>> GetAllBlogsAsync()
    {
        var blogs = await _context.BlogPosts
            .AsNoTracking()
            .Select(b => new GetBlogResponse(
                b.Id,
                b.Title,
                b.Content,
                b.Tags.Select(t => t.Tag).ToList(),
                b.CreatedAt
            ))
            .ToListAsync();
        
        return blogs;
    }

    public async Task<CreateBlogResponse> CreateBlogAsync(CreateBlogRequest dto)
    {
        TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
        DateTimeOffset phDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, targetZone);

        var blog = new Blog
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = phDateTime
        };

        _context.BlogPosts.Add(blog);
        await _context.SaveChangesAsync();

        return new CreateBlogResponse(
            blog.Id,
            blog.Title,
            blog.Content,
            blog.Tags,
            blog.CreatedAt
        );
    }
}