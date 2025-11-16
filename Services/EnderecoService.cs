using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace EnvioRapido.Api.Services;

public class EnderecoService : IEnderecoService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _cfg;

    public EnderecoService(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _httpFactory = httpFactory;
        _cfg = cfg;
    }

    public async Task<(bool ok, string? msg)> ValidarCepAsync(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8 || !cep.All(char.IsDigit))
            return (false, "CEP deve conter exatamente 8 números.");

        var baseUrl = _cfg["ViaCep:BaseUrl"] ?? "https://viacep.com.br/ws";

        try
        {
            var client = _httpFactory.CreateClient();
            var url = $"{baseUrl.TrimEnd('/')}/{cep}/json/";
            var resp = await client.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
                return (false, $"Erro ao consultar ViaCEP: {(int)resp.StatusCode}");

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("erro", out var erroProp) &&
                erroProp.ValueKind == JsonValueKind.True)
            {
                return (false, "CEP não encontrado no ViaCEP.");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao consultar ViaCEP: {ex.Message}");
        }
    }
}
