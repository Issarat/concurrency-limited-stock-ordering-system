using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LimitedStockOrdering.Application.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LimitedStockOrdering.Api.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions jwtOptions = options.Value;

    public AuthSessionResponse CreateSession(AuthUser user)
    {
        var lifetimeMinutes = user.IsAnonymous
            ? jwtOptions.GuestLifetimeMinutes
            : jwtOptions.AdminLifetimeMinutes;
        var expiresAt = DateTime.UtcNow.AddMinutes(lifetimeMinutes);
        var displayName = string.Join(
            ' ',
            new[] { user.FirstName, user.LastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("is_anonymous", user.IsAnonymous ? "true" : "false")
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Key));
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthSessionResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = new AuthUserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = string.IsNullOrWhiteSpace(displayName)
                    ? user.Username
                    : displayName,
                Role = user.Role,
                IsAnonymous = user.IsAnonymous
            }
        };
    }
}
