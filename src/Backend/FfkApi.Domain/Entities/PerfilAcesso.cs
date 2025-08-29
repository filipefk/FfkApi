namespace FfkApi.Domain.Entities;

public class PerfilAcesso : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public ICollection<Permissao> Permissoes { get; set; } = [];
}
