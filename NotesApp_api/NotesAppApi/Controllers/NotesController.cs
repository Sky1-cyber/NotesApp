using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAppApi.Extensions;
using NotesAppApi.Models.Dtos;
using NotesAppApi.Services;

namespace NotesAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _service;
    public NotesController(INoteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] NoteQueryDto query)
    {
        var userId = User.GetUserId();
        var (items, total) = await _service.GetAllAsync(userId, query);
        return Ok(new
        {
            items, total, page = query.Page, pageSize = query.PageSize
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id, User.GetUserId()));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNoteDto dto)
    {
        try
        {
            var note = await _service.CreateAsync(User.GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new
            {
                id = note.Id
            }, note);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateNoteDto dto)
    {
        try
        {
            return Ok(await _service.UpdateAsync(id, User.GetUserId(), dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id, User.GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}