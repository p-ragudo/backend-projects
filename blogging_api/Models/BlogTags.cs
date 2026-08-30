namespace blogging_api.Models;

public class BlogTags
{
    public int Id { get; set; }
    public string Tag { get; set;} = string.Empty;
    public List<Blog> Blogs { get; set; } = new();
}