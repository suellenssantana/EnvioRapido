using EnvioRapido.Api.Domain;
using EnvioRapido.Api.DTOs;
using EnvioRapido.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnvioRapido.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // In-memory user store for the challenge
    private static readonly List<User> _users = new();
    private readonly IJwtTokenService _jwtService;

    public AuthController(IJwtTokenService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (_users.Any(u => u.Email == request.Email))
            return BadRequest(new { message = "E-mail já cadastrado." });

        var user = new User
        {
            Nome = request.Nome,
            Email = request.Email,
            Password = request.Senha
        };

        _users.Add(user);

        return Ok(new { message = "Usuário registrado com sucesso." });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = _users.SingleOrDefault(u => u.Email == request.Email);
        if (user is null || user.Password != request.Senha)
            return Unauthorized(new { message = "Credenciais inválidas." });

        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponse(token));
    }
}
