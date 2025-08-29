namespace FfkApi.Communication.Requests;

public class RequestAtivarUsuario
{
    public string? TokenAtivacao { get; set; } = string.Empty;
    public string? Nome { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Cpf { get; set; } = string.Empty;
    public string? Senha { get; set; } = string.Empty;
}
