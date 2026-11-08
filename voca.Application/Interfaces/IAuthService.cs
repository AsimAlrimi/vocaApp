using voca.Application.DTOs.Auth;

namespace voca.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginWithGoogleAsync(string idToken);
}