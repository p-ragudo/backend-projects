using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace basic_auth.StudentService;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateStudentRequest request)
    {
        try
        {
            var response = await _studentService.Create(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Database write operation unsuccessful"
            );
        }
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromBody] GetStudentsQuery request)
    {
        try
        {
            var response = await _studentService.Get(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Error fetching students"
            );
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            var response = await _studentService.GetById(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Error fetching student"
            );
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        try
        {
            var response = await _studentService.Update(id, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Database write operation unsuccessful"
            );
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var response = await _studentService.Delete(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: ex.Message,
                title: "Database delete operation unsuccessful"
            );
        }
    }
}