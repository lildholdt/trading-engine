using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TradingEngine.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    public record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var configuredUsername = configuration["Auth:Username"] ?? string.Empty;
        var configuredPassword = configuration["Auth:Password"] ?? string.Empty;

        if (!string.Equals(request.Username, configuredUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, configuredPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var issuer = configuration["Auth:Issuer"] ?? "TradingEngine";
        var audience = configuration["Auth:Audience"] ?? "TradingEngine.Client";
        var jwtKey = configuration["Auth:JwtKey"] ?? "SuperSecretChangeMe1234567890123456";
        var expiresMinutes = int.TryParse(configuration["Auth:ExpiresMinutes"], out var minutes)
            ? minutes
            : 480;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "User")
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return Ok(new
        {
            token,
            username = request.Username,
            expiresAt
        });
    }
}
