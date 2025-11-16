using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace EnvioRapido.Api.Services;

public class MelhorEnvioGateway : IFreteGateway
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _cfg;

    public MelhorEnvioGateway(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _httpFactory = httpFactory;
        _cfg = cfg;
    }

    public async Task<(bool ok, string? msg, decimal valor, int prazoDias)> CalcularAsync(
        string remetenteCep,
        string destinatarioCep,
        decimal pesoKg,
        int alturaCm,
        int larguraCm,
        int comprimentoCm)
    {
        var baseUrl = _cfg["MelhorEnvio:BaseUrl"] ?? "https://sandbox.melhorenvio.com.br/api/v2";
        var token = _cfg["MelhorEnvio:Token"];

        if (string.IsNullOrWhiteSpace(token))
            return (false, "Token do Melhor Envio não configurado.", 0m, 0);

        try
        {
            var client = _httpFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new
            {
                from = new { postal_code = remetenteCep },
                to = new { postal_code = destinatarioCep },
                packages = new[]
                {
                    new
                    {
                        width  = larguraCm,
                        height = alturaCm,
                        length = comprimentoCm,
                        weight = pesoKg,
                        insurance = 0.0m
                    }
                },
                options = new
                {
                    receipt  = false,
                    own_hand = false
                },
                services = (string?)null
            };

            var response = await client.PostAsJsonAsync("me/shipment/calculate", body);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return (false, $"Erro Melhor Envio: {(int)response.StatusCode} - {errorText}", 0m, 0);
            }

        

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                    return (false, "Nenhuma cotação retornada pelo Melhor Envio.", 0m, 0);

                var first = root[0];

        
                decimal price = 0m;
                if (first.TryGetProperty("price", out var priceProp))
                {
                    if (priceProp.ValueKind == JsonValueKind.Number)
                    {
                        price = priceProp.GetDecimal();
                    }
                    else if (priceProp.ValueKind == JsonValueKind.String)
                    {
                        var s = priceProp.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            decimal.TryParse(
                                s,
                                NumberStyles.Any,
                                CultureInfo.InvariantCulture,
                                out price);
                        }
                    }
                }

    
                int dias = 0;

                if (TryGetDeliveryDays(first, out var d))
                    dias = d;

                return (true, null, price, dias);
            }
            catch (Exception exJson)
            {
                return (false, $"Erro ao interpretar resposta do Melhor Envio: {exJson.Message}", 0m, 0);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao chamar Melhor Envio: {ex.Message}", 0m, 0);
        }
    }

    private static bool TryGetDeliveryDays(JsonElement quote, out int days)
    {
        days = 0;

        if (TryGetDaysFromProperty(quote, "delivery_time", out days)) return true;
        if (TryGetDaysFromProperty(quote, "_delivery_time", out days)) return true;
        if (TryGetDaysFromProperty(quote, "delivery", out days)) return true;
        if (TryGetDaysFromProperty(quote, "delivery_range", out days)) return true;

        return false;
    }

    private static bool TryGetDaysFromProperty(JsonElement quote, string propName, out int days)
    {
        days = 0;

        if (!quote.TryGetProperty(propName, out var deliveryProp))
            return false;

        if (deliveryProp.ValueKind == JsonValueKind.Object)
        {
            if (deliveryProp.TryGetProperty("days", out var daysProp))
            {
                if (daysProp.ValueKind == JsonValueKind.Number)
                {
                    days = daysProp.GetInt32();
                    return true;
                }

                if (daysProp.ValueKind == JsonValueKind.String)
                {
                    var s = daysProp.GetString();
                    if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var d))
                    {
                        days = d;
                        return true;
                    }
                }
            }
        }

       
        if (deliveryProp.ValueKind == JsonValueKind.Number)
        {
            days = deliveryProp.GetInt32();
            return true;
        }

        if (deliveryProp.ValueKind == JsonValueKind.String)
        {
            var s = deliveryProp.GetString();
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var d))
            {
                days = d;
                return true;
            }
        }

        return false;
    }
}
