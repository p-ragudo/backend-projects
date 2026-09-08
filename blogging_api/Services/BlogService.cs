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

    public async Task<BlogResponse?> CreateBlogAsync(CreateBlogRequest dto)
    {
        TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
        DateTimeOffset phDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, targetZone);

        var incomingTagNames = dto.Tags
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        var existingTags = await _context.BlogTags
            .Where(t => incomingTagNames.Contains(t.Tag))
            .ToListAsync();
        
        var existingTagNames = existingTags
            .Select(t => t.Tag.ToLowerInvariant())
            .ToHashSet();
        
        var newTags = incomingTagNames
            .Where(name => !existingTagNames.Contains(name.ToLowerInvariant()))
            .Select(name => new BlogTag { Tag = name})
            .ToList();

        var allTags = existingTags.Concat(newTags).ToList();

        var blog = new Blog
        {
            Title = dto.Title,
            Content = dto.Content,
            Tags = allTags,
            CreatedAt = phDateTime
        };

        _context.BlogPosts.Add(blog);
        await _context.SaveChangesAsync();

        return new BlogResponse(
            blog.Id,
            blog.Title,
            blog.Content,
            blog.Tags.Select(t => t.Tag).ToList(),
            blog.CreatedAt,
            blog.EditedAt
        );
    }

    public async Task<List<BlogResponse>?> GetAllBlogsAsync()
    {
        var blogs = await _context.BlogPosts
            .AsNoTracking()
            .Select(b => new BlogResponse(
                b.Id,
                b.Title,
                b.Content,
                b.Tags.Select(t => t.Tag).ToList(),
                b.CreatedAt,
                b.EditedAt
            ))
            .ToListAsync();
        
        return blogs;
    }

    public async Task<BlogResponse?> UpdateBlogAsync(UpdateBlogRequest dto) 
    {
        var blog = await _context.BlogPosts
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(t => t.Id == dto.Id);

        if (blog == null)
        {
            return null;
        }

        blog.Title = dto.Title ?? blog.Title;
        blog.Content = dto.Content ?? blog.Content;

        if (dto.Tags is not null)
        {
            var existingTags = await _context.BlogTags
                .Where(t => dto.Tags.Contains(t.Tag))
                .ToListAsync();
            
            blog.Tags = dto.Tags.Select(tagName =>
                existingTags.FirstOrDefault(t => t.Tag == tagName)
                ?? new BlogTag{ Tag = tagName}
            ).ToList();
        }

        TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
        DateTimeOffset phDateTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, targetZone);
        blog.EditedAt = phDateTime;

        await _context.SaveChangesAsync();

        var blogResponse = new BlogResponse
        (
            blog.Id,
            blog.Title,
            blog.Content,
            blog.Tags.Select(t => t.Tag).ToList(),
            blog.CreatedAt,
            blog.EditedAt
        );

        return blogResponse;
    }

    public async Task<bool> DeleteBlogByIdAsync(int id)
    {
        var blog = await _context.BlogPosts.FirstOrDefaultAsync(t => t.Id == id);
        if (blog is null) return false;

        _context.BlogPosts.Remove(blog);
        await _context.SaveChangesAsync();

        return true;
    }
}