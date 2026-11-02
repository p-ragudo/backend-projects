using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApi.Services.Expenses;

[ApiController]
[Authorize]
[Route("api/expenses")]
public class ExpensesController(
    ExpenseService _expenseService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var result = await _expenseService.CreateAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error.ToString());
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occurred."
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetExpenses(CancellationToken ct)
    {
        try
        {
            var result = await _expenseService.GetAsync(ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error.ToString());
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occurred."
            });
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateExpense([FromBody] UpdateExpenseRequest request)
    {
        try
        {
            var result = await _expenseService.UpdateAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error.ToString());
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occurred."
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteExpense([FromQuery] string id)
    {
        try
        {
            var result = await _expenseService.DeleteAsync(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return Problem(result.Error.ToString());
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occurred."
            });
        }
    }
}