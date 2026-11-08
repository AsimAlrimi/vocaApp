using Microsoft.AspNetCore.Mvc;
using voca.Application.DTOs.Auth;
using voca.Application.Interfaces;

namespace voca.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> GoogleLogin(
        GoogleLoginRequest request)
    {
        var response =
            await _authService.LoginWithGoogleAsync(
                request.IdToken);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid Google ID token."
            });
        }

        return Ok(response);
    }
}