using System.ComponentModel.DataAnnotations;

namespace EnvioRapido.Api.DTOs;

public class EnvioCreateRequest
{
    [Required]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP de remetente deve conter exatamente 8 números.")]
    public string RemetenteCep { get; set; } = default!;

    [Required]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP de destinatário deve conter exatamente 8 números.")]
    public string DestinatarioCep { get; set; } = default!;

    [Required]
    [Range(0.001, double.MaxValue, ErrorMessage = "Peso deve ser maior que zero.")]
    public decimal PesoKg { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Altura deve ser maior que zero.")]
    public int AlturaCm { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Largura deve ser maior que zero.")]
    public int LarguraCm { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Comprimento deve ser maior que zero.")]
    public int ComprimentoCm { get; set; }
}

public record EnvioCadastroResponse(
    string RemetenteCep,
    string DestinatarioCep,
    decimal PesoKg,
    int AlturaCm,
    int LarguraCm,
    int ComprimentoCm,
    decimal ValorFrete,
    int PrazoDias);
