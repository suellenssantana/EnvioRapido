using System.ComponentModel.DataAnnotations;

namespace EnvioRapido.Api.DTOs;

public class RegisterRequest
{
    [Required]
    public string Nome { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = default!;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string Senha { get; set; } = default!;
}

public record AuthResponse(string Token);
