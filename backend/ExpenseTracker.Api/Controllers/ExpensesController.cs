using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.DTOs.Expense;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpensesController(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
        return userId;
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var userId = GetUserId();
        var expenses = await _expenseRepository.GetExpensesByUserIdAsync(userId);
        
        var dtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Amount = e.Amount,
            Category = e.Category,
            Date = e.Date,
            Notes = e.Notes
        });

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        var userId = GetUserId();
        var expense = new Expense
        {
            UserId = userId,
            Amount = request.Amount,
            Category = request.Category,
            Date = request.Date,
            Notes = request.Notes
        };

        var createdExpense = await _expenseRepository.CreateExpenseAsync(expense);

        var dto = new ExpenseDto
        {
            Id = createdExpense.Id,
            Amount = createdExpense.Amount,
            Category = createdExpense.Category,
            Date = createdExpense.Date,
            Notes = createdExpense.Notes
        };

        return CreatedAtAction(nameof(GetExpenses), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var userId = GetUserId();
        
        // Strictly speaking, we could verify existence first, but the repository SP filters by UserId anyway.
        // If it doesn't exist or belong to the user, no rows update.
        
        var expense = new Expense
        {
            Id = id,
            UserId = userId,
            Amount = request.Amount,
            Category = request.Category,
            Date = request.Date,
            Notes = request.Notes
        };

        await _expenseRepository.UpdateExpenseAsync(expense);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var userId = GetUserId();
        await _expenseRepository.DeleteExpenseAsync(id, userId);
        return NoContent();
    }
}
