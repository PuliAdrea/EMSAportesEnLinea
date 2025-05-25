using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.Application.Interfaces;
using EMS.Domain.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EMS.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", PasswordHash = "admin123", Role = "Admin" }
    };

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var user = _users.SingleOrDefault(u => u.Username == username && u.PasswordHash == password);

        if (user == null) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!)),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Task<User?> GetUserByIdAsync(int id)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
}