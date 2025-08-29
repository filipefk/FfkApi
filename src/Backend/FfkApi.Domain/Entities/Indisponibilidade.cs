namespace FfkApi.Domain.Entities;

public class Indisponibilidade : EntityBase
{
    public string Descricao { get; set; } = string.Empty;
    public DateOnly DataInicial { get; set; }
    public DateOnly DataFinal { get; set; }
    public Guid IdUsuario { get; set; }
    public Usuario Usuario { get; set; } = default!;
}
