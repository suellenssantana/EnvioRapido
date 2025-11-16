using EnvioRapido.Api.Domain;

namespace EnvioRapido.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
