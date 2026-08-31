namespace blogging_api.Models;

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }= string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<BlogTag> Tags { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
}