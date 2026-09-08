using blogging_api.Dtos;
using blogging_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogging_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController : ControllerBase
{
    private readonly BlogService _service;

    public BlogsController(BlogService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<BlogResponse>> CreateBlog([FromBody] CreateBlogRequest request)
    {
        var createdBlog = await _service.CreateBlogAsync(request);
        return createdBlog;
    }

    [HttpGet]
    public async Task<ActionResult<BlogResponse>> GetAllBlogs()
    {
        var blogs = await _service.GetAllBlogsAsync();
        return Ok(blogs);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BlogResponse>> UpdateBlog(
        [FromRoute] int id,
        [FromBody] UpdateBlogRequest request
    )
    {
        var result = await _service.UpdateBlogAsync(request);
        
        if (result is not null)
        {
            return result;
        }

        return NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBlogById([FromRoute] int id)
    {
        var success = await _service.DeleteBlogByIdAsync(id);

        if (success)
        {
            return Ok();
        }

        return Problem();
    }
}