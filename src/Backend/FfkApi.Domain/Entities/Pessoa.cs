using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class Pessoa : EntityBase
{
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Indefinida;
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public Guid IdOrganizacao { get; set; } = default!;
    public Organizacao Organizacao { get; set; } = default!;
}
