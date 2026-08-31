namespace blogging_api.Dtos;

public record BlogResponse(
    int Id,
    string Title,
    string Content,
    List<string>? Tags,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt
);