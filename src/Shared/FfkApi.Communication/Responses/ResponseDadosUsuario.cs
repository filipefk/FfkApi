namespace FfkApi.Communication.Responses;

public class ResponseDadosUsuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; } = null;
    public string Status { get; set; } = string.Empty;
    public string Organizacao { get; set; } = string.Empty;
    public IList<string> PerfisAcesso { get; set; } = [];
    public IList<string> Permissoes { get; set; } = [];
}
