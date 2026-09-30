namespace ExpenseTracker.Infrastructure.Data;

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ExpenseTracker.Core.Entities;
using ExpenseTracker.Core.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string is missing");
    }

    public async Task<User> CreateUserAsync(string email, string passwordHash)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CreateUser", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
        
        var newIdParam = new SqlParameter("@NewId", SqlDbType.UniqueIdentifier)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(newIdParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        var newId = (Guid)newIdParam.Value;
        
        return new User
        {
            Id = newId,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetUserByEmail", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        return null;
    }
}
