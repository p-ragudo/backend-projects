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

    public async Task<List<BlogResponse>?> GetBlogsAsync(BlogQueryParams query)
    {
        if (query.Terms == null && query.Tags == null)
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

        var blogsQuery = _context.BlogPosts
            .AsNoTracking()
            .AsQueryable();

        if (query.Terms is { Count: > 0})
        {
            var terms = query.Terms
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();

            if (terms.Count > 0)
            {
                blogsQuery = blogsQuery.Where(b =>
                    terms.Any(term =>
                        b.Title.ToLower().Trim().Contains(term) ||
                        b.Content.ToLower().Trim().Contains(term))); 
            }
        }

        if (query.Tags is { Count: > 0})
        {
            var targetTags = query.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();
            
            if (targetTags.Count > 0)
            {
                blogsQuery = blogsQuery.Where(b =>
                    b.Tags.Any(t => targetTags.Contains(t.Tag.Trim().ToLower())));
            }
        }

        var results = await blogsQuery
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BlogResponse(
                b.Id,
                b.Title,
                b.Content,
                b.Tags.Select(t => t.Tag).ToList(),
                b.CreatedAt,
                b.EditedAt
            ))
            .ToListAsync();

        return results;
    }

    public async Task<BlogResponse?> GetBlogById(int id)
    {
        var blog = await _context.BlogPosts
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (blog == null)
        {
            return null;
        }
        
        var response = new BlogResponse(
            blog.Id,
            blog.Title,
            blog.Content,
            blog.Tags.Select(b => b.Tag).ToList(),
            blog.CreatedAt,
            blog.EditedAt
        );

        return response;
    }

    public async Task<BlogResponse?> UpdateBlogAsync(int id, UpdateBlogRequest dto) 
    {
        var blog = await _context.BlogPosts
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(t => t.Id == id);

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