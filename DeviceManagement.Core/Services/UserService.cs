using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeviceManagement.Core.DTOs.User;
using DeviceManagement.Infrastructure.Entities;
using DeviceManagement.Core.Services.Interfaces;
using DeviceManagement.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManagement.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public UserService(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userRepo.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Location = dto.Location,
            Role = "User"
        };

        var created = await _userRepo.CreateAsync(user);

        return new UserDto
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            Role = created.Role,
            Location = created.Location
        };
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower().Trim())
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!validPassword)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("name", user.Name),
            new Claim("role", user.Role),
            new Claim("location", user.Location)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}