using blogging_api.Models;

namespace blogging_api.Dtos;

public record CreateBlogResponse(
    int Id,
    string Title,
    string Content,
    List<BlogTags> Tags,
    DateTimeOffset CreatedAt
);

public record GetBlogResponse(
    int Id,
    string Title,
    string Content,
    List<string>? Tags,
    DateTimeOffset CreatedAt
);