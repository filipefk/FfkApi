namespace FfkApi.Domain.Entities;

public class Fila : EntityBase
{
    public Guid? IdEquipe { get; set; } = null!;
    public Equipe? Equipe { get; set; } = null!;
    public Guid? IdUsuario { get; set; } = null!;
    public Usuario? Usuario { get; set; } = null!;
    public ICollection<FilaItem> FilaItens { get; set; } = [];
}
