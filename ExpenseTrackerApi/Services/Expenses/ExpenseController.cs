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

            if (result.IsFailure)
            {
                return result.Error switch
                {
                    
                };
            }

            return Ok(result.Value);
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