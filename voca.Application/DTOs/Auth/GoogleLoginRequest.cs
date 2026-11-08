using System.ComponentModel.DataAnnotations;

namespace voca.Application.DTOs.Auth;

public class GoogleLoginRequest
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
}