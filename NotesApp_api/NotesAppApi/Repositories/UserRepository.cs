using System.Data;
using Dapper;
using NotesAppApi.Data;
using NotesAppApi.Models.Entities;

namespace NotesAppApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int id);
}

public class UserRepository : IUserRepository
{
    public readonly IDbConnectionFactory _factory;
    public UserRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
            SELECT id, username, password_hash AS PasswordHash, created_at AS CreatedAt 
            FROM users WHERE username = @Username
        ";
        return await conn.QuerySingleOrDefaultAsync<User?>(sql, new { Username = username });
    }

    public async Task<User> CreateAsync(User user)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
        INSERT INTO users (username, password_hash)
        OUTPUT INSERTED.id,
               INSERTED.username,
               INSERTED.password_hash AS PasswordHash,
               INSERTED.created_at    AS CreatedAt
        VALUES (@Username, @PasswordHash);
    ";
        return await conn.QuerySingleAsync<User>(sql, user);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
            SELECT id, username, password_hash AS PasswordHash, created_at AS CreatedAt 
            FROM users WHERE id = @Id
        ";
        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }
}