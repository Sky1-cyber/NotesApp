using System.ComponentModel.DataAnnotations;

namespace NotesAppApi.Models.Dtos;

public class CreateNoteDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class UpdateNoteDto
{
    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class NoteQueryDto
{
    public string? Search { get; set; }
    public string? SortBy { get; set; } = "updatedAt";
    public string? SortDir { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}