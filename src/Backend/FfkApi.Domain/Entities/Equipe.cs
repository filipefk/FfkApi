using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class Equipe : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public StatusEquipe Status { get; set; } = StatusEquipe.Indefinido;
    public ICollection<MembroEquipe> Membros { get; set; } = [];
    public Fila? Fila { get; set; } = null;
    public Guid IdOrganizacao { get; set; } = default!;
    public Organizacao Organizacao { get; set; } = default!;
}
