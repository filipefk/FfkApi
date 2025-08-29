namespace FfkApi.Domain.Entities;

public class MembroEquipe : EntityBase
{
    public bool Lider { get; set; } = false;
    public Guid IdEquipe { get; set; }
    public Guid IdUsuario { get; set; }
    public Equipe Equipe { get; set; } = default!;
    public Usuario Usuario { get; set; } = default!;
}
