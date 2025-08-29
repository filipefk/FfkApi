namespace FfkApi.Communication.Requests;

public class RequestCadastrarUsuario
{
    public string? Nome { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; } = null;
    public string? Organizacao { get; set; } = null;
    public IList<string>? PerfisAcesso { get; set; } = null;
    public IList<string>? Permissoes { get; set; } = null;
}
