namespace EnvioRapido.Api.Services;

public interface IFreteGateway
{
    Task<(bool ok, string? msg, decimal valor, int prazoDias)> CalcularAsync(
        string remetenteCep,
        string destinatarioCep,
        decimal pesoKg,
        int alturaCm,
        int larguraCm,
        int comprimentoCm);
}
