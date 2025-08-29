namespace FfkApi.Domain.Entities;

public class RefreshToken : EntityBase
{
    public string Valor { get; set; } = string.Empty;
    public Guid IdUsuario { get; set; }
    public Usuario Usuario { get; set; } = default!;
}
