using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotesAppApi.Models.Dtos;
using NotesAppApi.Models.Entities;
using NotesAppApi.Repositories;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NotesAppApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 3)
            throw new ArgumentException("Username must be at least 3 chars");
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 chars");

        if (await _users.GetByUsernameAsync(dto.Username) is not null)
            throw new InvalidOperationException("Username already taken");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        var created = await _users.CreateAsync(user);
        return new AuthResponseDto(GenerateToken(created), created.Username, created.Id);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByUsernameAsync(dto.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return new AuthResponseDto(GenerateToken(user), user.Username, user.Id);
    }

    private string GenerateToken(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}