using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class Feed : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? PalavrasChave { get; set; } = string.Empty;
    public StatusFeed Status { get; set; } = StatusFeed.Indefinido;
    public IList<Anexo> Anexos { get; set; } = [];
    public IList<Usuario> VisibilidadeUsuarios { get; set; } = [];
    public IList<Equipe> VisibilidadeEquipes { get; set; } = [];
    public DateOnly? ExpiraEm { get; set; } = null;
    public Guid IdOrganizacao { get; set; } = default!;
    public Organizacao Organizacao { get; set; } = default!;
}
