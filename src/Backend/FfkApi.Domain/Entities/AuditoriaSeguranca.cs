namespace FfkApi.Domain.Entities;

public class AuditoriaSeguranca : EntityBase
{
    public string Evento { get; set; } = null!; // e.g., "AuthFailed", "RateLimit", "CorsBlocked"
    public string? Usuario { get; set; }
    public string? EnderecoIp { get; set; }
    public string? Caminho { get; set; }
    public string? Metodo { get; set; }
    public string? Detalhes { get; set; }
}
