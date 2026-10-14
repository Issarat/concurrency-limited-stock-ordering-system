using System.Security.Claims;
using LimitedStockOrdering.Api.Authentication;
using LimitedStockOrdering.Application.Authentication;
using LimitedStockOrdering.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LimitedStockOrdering.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("Authentication")]
public sealed class AuthenticationController(
    IAuthenticationRepository authenticationRepository,
    JwtTokenService jwtTokenService,
    PasswordHashService passwordHashService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("guest")]
    public async Task<ActionResult<AuthSessionResponse>> CreateGuest(
        CancellationToken cancellationToken)
    {
        var guest = await authenticationRepository.CreateGuestAsync(cancellationToken);
        return Ok(jwtTokenService.CreateSession(guest));
    }

    [AllowAnonymous]
    [HttpPost("admin/login")]
    public async Task<ActionResult<AuthSessionResponse>> LoginAdmin(
        [FromBody] AdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        if (string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(CreateProblem(
                "กรุณากรอกชื่อผู้ใช้และรหัสผ่าน",
                StatusCodes.Status400BadRequest));
        }

        var admin = await authenticationRepository.FindAdminByUsernameAsync(
            username,
            cancellationToken);

        if (admin is null
            || !passwordHashService.Verify(request.Password, admin.PasswordHash))
        {
            return Unauthorized(CreateProblem(
                "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง",
                StatusCodes.Status401Unauthorized));
        }

        return Ok(jwtTokenService.CreateSession(admin));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthUserResponse> Me()
    {
        if (!Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Unauthorized();
        }

        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var firstName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        var lastName = User.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        var isAnonymous = string.Equals(
            User.FindFirstValue("is_anonymous"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        return Ok(new AuthUserResponse
        {
            UserId = userId,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            DisplayName = string.Join(
                ' ',
                new[] { firstName, lastName }
                    .Where(value => !string.IsNullOrWhiteSpace(value))),
            Role = role,
            IsAnonymous = isAnonymous
        });
    }

    private static ProblemDetails CreateProblem(string detail, int status) => new()
    {
        Status = status,
        Title = "ไม่สามารถเข้าสู่ระบบได้",
        Detail = detail
    };
}
