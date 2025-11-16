namespace EnvioRapido.Api.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!; // plain for challenge only
}
