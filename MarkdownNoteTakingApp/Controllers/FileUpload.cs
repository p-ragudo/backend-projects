using Microsoft.AspNetCore.Mvc;

namespace MarkdownNoteTakingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null | file.Length == 0)
            return BadRequest("No file uploaded");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { file.FileName, file.Length });
    }
}