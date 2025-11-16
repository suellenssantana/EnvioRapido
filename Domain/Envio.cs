namespace EnvioRapido.Api.Domain;

public class Envio
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string RemetenteCep { get; set; } = default!;
    public string DestinatarioCep { get; set; } = default!;

    public decimal PesoKg { get; set; }
    public int AlturaCm { get; set; }
    public int LarguraCm { get; set; }
    public int ComprimentoCm { get; set; }

    public decimal ValorFrete { get; set; }
    public int PrazoDias { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
