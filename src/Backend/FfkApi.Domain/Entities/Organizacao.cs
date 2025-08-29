namespace FfkApi.Domain.Entities;

public class Organizacao : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? RemetenteEmail { get; set; } = null;
    public string? RemetenteNome { get; set; } = null;
    public string? ModeloEmailAtivacao { get; set; } = null;
    public string? ModeloEmailNovaSenha { get; set; } = null;
}
