namespace FfkApi.Domain.Entities;

public class TokenNovaSenha : EntityBase
{
    public string Valor { get; set; } = string.Empty;
    public Guid IdUsuario { get; set; }
    public Usuario Usuario { get; set; } = default!;
    public bool EmailEnviado { get; set; } = false;
    public DateTime? UltimaTentativaEnvioEmail { get; set; } = null;
    public string? ErroEnvioEmail { get; set; } = null;
}
