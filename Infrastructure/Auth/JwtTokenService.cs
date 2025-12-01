using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public interface IJwtTokenService
{
    (string token, DateTime expiresUtc) Create(Guid userId, string email, string displayName);
}

internal sealed class JwtTokenService(IConfiguration cfg) : IJwtTokenService
{
    public (string token, DateTime expiresUtc) Create(Guid userId, string email, string displayName)
    {
        var key = cfg["Jwt:SigningKey"] ?? Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("JWT_SIGNING_KEY missing");

        var exp = DateTime.UtcNow.AddHours(12);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("name", displayName)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(jwt), exp);
    }
}