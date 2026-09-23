using Dapper;
using NotesAppApi.Data;
using NotesAppApi.Models.Dtos;
using NotesAppApi.Models.Entities;

namespace NotesAppApi.Repositories;

public interface INoteRepository
{
    Task<(IEnumerable<Note> Items, int Total)> GetPagedAsync(int userId, NoteQueryDto q);
    Task<Note?> GetByIdAsync(int id, int userId);
    Task<Note> CreateAsync(Note note);
    Task<Note?> UpdateAsync(int id, int userId, string title, string? content);
    Task<bool> DeleteAsync(int id, int userId);
}

public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _factory;
    public NoteRepository(IDbConnectionFactory factory) => _factory = factory;

    private const string SelectCols = "id, user_id AS UserId, title, content, created_at AS CreatedAt, updated_at AS UpdatedAt";

    public async Task<(IEnumerable<Note> Items, int Total)> GetPagedAsync(int userId, NoteQueryDto q)
    {
        var sortCol = q.SortBy?.ToLower() switch
        {
            "title" => "title",
            "createdat" => "created_at",
            _ => "updated_at"
        };
        var sortDir = q.SortDir?.ToLower() == "asc" ? "ASC" : "DESC";
        var offset = (q.Page - 1) * q.PageSize;

        using var conn = _factory.CreateConnection();

        var where = "WHERE user_id = @UserId";
        if (!string.IsNullOrWhiteSpace(q.Search))
            where += " AND (title LIKE @Search OR COALESCE(content,'') LIKE @Search)";

        var countSql = $"SELECT COUNT(*) FROM notes {where}";
        var dataSql = $@"
            SELECT {SelectCols}
            FROM notes {where}
            ORDER BY {sortCol} {sortDir}
            OFFSET @Offset ROWS
            FETCH NEXT @Limit ROWS ONLY
        ";

        var parameters = new
        {
            UserId = userId,
            Search = $"%{q.Search}%",
            Limit = q.PageSize,
            Offset = offset
        };

        var total = await conn.ExecuteScalarAsync<int>(countSql, parameters);
        var items = await conn.QueryAsync<Note>(dataSql, parameters);
        return (items, total);
    }

    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            SELECT {SelectCols} FROM notes WHERE id = @Id AND user_id = @UserId
        ";
        return await conn.QuerySingleOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
    }

    public async Task<Note> CreateAsync(Note note)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
        INSERT INTO notes (user_id, title, content)
        OUTPUT INSERTED.id,
               INSERTED.user_id    AS UserId,
               INSERTED.title,
               INSERTED.content,
               INSERTED.created_at AS CreatedAt,
               INSERTED.updated_at AS UpdatedAt
        VALUES (@UserId, @Title, @Content);
    ";
        return await conn.QuerySingleAsync<Note>(sql, note);
    }

    public async Task<Note?> UpdateAsync(int id, int userId, string title, string? content)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
        UPDATE notes
        SET title = @Title,
            content = @Content,
            updated_at = SYSUTCDATETIME()
        OUTPUT INSERTED.id,
               INSERTED.user_id    AS UserId,
               INSERTED.title,
               INSERTED.content,
               INSERTED.created_at AS CreatedAt,
               INSERTED.updated_at AS UpdatedAt
        WHERE id = @Id AND user_id = @UserId;
    ";
        return await conn.QuerySingleOrDefaultAsync<Note>(sql, new
        {
            Id = id, UserId = userId, Title = title, Content = content
        });
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        const string sql = "DELETE FROM notes WHERE id = @Id AND user_id = @UserId";
        var rows = await conn.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rows > 0;
    }
}