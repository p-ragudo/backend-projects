using blogging_api.Dtos;
using blogging_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogging_api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly BlogService _service;

    public BlogsController(BlogService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<GetBlogResponse>> GetAllBlogs()
    {
        var blogs = await _service.GetAllBlogsAsync();
        return Ok(blogs);
    }

    [HttpPost]
    public async Task<ActionResult<CreateBlogResponse>> CreateBlog([FromBody] CreateBlogRequest request)
    {
        var createdBlog = await _service.CreateBlogAsync(request);
        return createdBlog;
    }
}