namespace EnvioRapido.Api.Services;

public interface IEnderecoService
{
    Task<(bool ok, string? msg)> ValidarCepAsync(string cep);
}
