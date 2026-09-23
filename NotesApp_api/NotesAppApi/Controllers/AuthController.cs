using Microsoft.AspNetCore.Mvc;
using NotesAppApi.Models.Dtos;
using NotesAppApi.Services;

namespace NotesAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        try
        {
            return Ok(await _authService.RegisterAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });

        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        try
        {
            return Ok(await _authService.LoginAsync(dto));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
}