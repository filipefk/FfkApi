namespace FfkApi.Communication.Requests;

public class RequestAlterarUsuario
{
    public string? Id { get; set; } = string.Empty;
    public string? Nome { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; } = null;
    public string? Status { get; set; } = string.Empty;
    public string? Organizacao { get; set; } = string.Empty;
}
