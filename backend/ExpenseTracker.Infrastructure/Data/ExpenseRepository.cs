namespace ExpenseTracker.Infrastructure.Data;

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;

public class ExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;

    public ExpenseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string is missing");
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CreateExpense", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@UserId", expense.UserId);
        command.Parameters.AddWithValue("@Amount", expense.Amount);
        command.Parameters.AddWithValue("@Category", expense.Category);
        command.Parameters.AddWithValue("@Date", expense.Date);
        command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(expense.Notes) ? DBNull.Value : expense.Notes);

        var newIdParam = new SqlParameter("@NewId", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(newIdParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        expense.Id = (Guid)newIdParam.Value;
        return expense;
    }

    public async Task<IEnumerable<Expense>> GetExpensesByUserIdAsync(Guid userId)
    {
        var expenses = new List<Expense>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetExpensesByUserId", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            expenses.Add(new Expense
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            });
        }

        return expenses;
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_UpdateExpense", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Id", expense.Id);
        command.Parameters.AddWithValue("@UserId", expense.UserId);
        command.Parameters.AddWithValue("@Amount", expense.Amount);
        command.Parameters.AddWithValue("@Category", expense.Category);
        command.Parameters.AddWithValue("@Date", expense.Date);
        command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(expense.Notes) ? DBNull.Value : expense.Notes);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteExpenseAsync(Guid id, Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_DeleteExpense", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
}
