using NotesAppApi.Models.Dtos;
using NotesAppApi.Models.Entities;
using NotesAppApi.Repositories;

namespace NotesAppApi.Services;

public interface INoteService
{
    Task<(IEnumerable<Note> Items, int Total)> GetAllAsync(int userId, NoteQueryDto q);
    Task<Note> GetByIdAsync(int id, int userId);
    Task<Note> CreateAsync(int userId, CreateNoteDto dto);
    Task<Note> UpdateAsync(int id, int userId, UpdateNoteDto dto);
    Task DeleteAsync(int id, int userId);
}

public class NoteService : INoteService
{
    private readonly INoteRepository _repo;
    public NoteService(INoteRepository repo) => _repo = repo;

    public Task<(IEnumerable<Note> Items, int Total)> GetAllAsync(int userId, NoteQueryDto q) => _repo.GetPagedAsync(userId, q);

    public async Task<Note> GetByIdAsync(int id, int userId) => await _repo.GetByIdAsync(id, userId) ?? throw new KeyNotFoundException("Note not found");

    public async Task<Note> CreateAsync(int userId, CreateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        return await _repo.CreateAsync(new Note
        {
            UserId = userId,
            Title = dto.Title.Trim(),
            Content = dto.Content
        });
    }

    public async Task<Note> UpdateAsync(int id, int userId, UpdateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        return await _repo.UpdateAsync(id, userId, dto.Title.Trim(), dto.Content) ?? throw new KeyNotFoundException("Note not found");
    }

    public async Task DeleteAsync(int id, int userId)
    {
        if (!await _repo.DeleteAsync(id, userId))
            throw new KeyNotFoundException("Note not found");
    }
}