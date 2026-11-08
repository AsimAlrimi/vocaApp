using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using voca.Application.DTOs.Auth;
using voca.Application.Interfaces;
using voca.Domain.Entities;
using voca.Infrastructure.Data;

namespace voca.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginWithGoogleAsync(string idToken)
    {
        var googleClientId =
            _configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException(
                "Google Client ID is not configured.");

        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });
        }
        catch (InvalidJwtException)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(payload.Subject) ||
            string.IsNullOrWhiteSpace(payload.Email))
        {
            return null;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(user =>
                user.GoogleId == payload.Subject);

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                GoogleId = payload.Subject,
                Email = payload.Email,
                Name = payload.Name ?? payload.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
        }
        else
        {
            // Keep profile information synchronized with Google.
            user.Email = payload.Email;
            user.Name = payload.Name ?? user.Name;
        }

        await _context.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT signing key is not configured.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var expiryMinutes =
            _configuration.GetValue<int>("Jwt:ExpiryMinutes");

        if (expiryMinutes <= 0)
        {
            expiryMinutes = 60;
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                ClaimTypes.Name,
                user.Name),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var keyBytes = Convert.FromBase64String(jwtKey);

        var signingKey =
            new SymmetricSecurityKey(keyBytes);

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponse
        {
            AccessToken =
                new JwtSecurityTokenHandler().WriteToken(token),

            ExpiresAt = expiresAt,

            User = new AuthUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        };
    }
}