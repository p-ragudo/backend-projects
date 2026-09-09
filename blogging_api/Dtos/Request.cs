using System.ComponentModel.DataAnnotations;
using blogging_api.Models;

namespace blogging_api.Dtos;

public record CreateBlogRequest(
    [Required(ErrorMessage = "Title is required")]
    string Title,
    string Content,
    List<string> Tags
);

public record UpdateBlogRequest(
    string? Title,
    string? Content,
    List<string>? Tags
);

public record BlogQueryParams(
    List<string>? Terms = null,
    List<string>? Tags = null
);