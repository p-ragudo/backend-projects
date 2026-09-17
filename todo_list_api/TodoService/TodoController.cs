using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace todo_list_api.TodoService;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] TodoRequest request)
    {
        var result = await _service.Create(request);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id},
            result
        );
    }

    [HttpGet]
    [ProducesResponseType(typeof(TodoQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TodoQueryResponse>> Get([FromQuery] TodoQueryRequest request)
    {
        var result = await _service.Get(request);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> GetById(int id)
    {
        var result = await _service.GetById(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoEditResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoEditResponse>> Put(
        int id, 
        [FromBody] TodoEditRequest request
    )
    {
        var result = await _service.UpdateById(id, request);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(TodoDeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDeleteResponse>> DeleteById(int id)
    {
        var result = await _service.DeleteById(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

}