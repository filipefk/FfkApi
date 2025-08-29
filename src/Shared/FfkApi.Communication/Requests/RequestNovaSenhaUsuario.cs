namespace FfkApi.Communication.Requests;

public class RequestNovaSenhaUsuario
{
    public string? TokenNovaSenha { get; set; } = string.Empty;
    public string? Nome { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Cpf { get; set; } = string.Empty;
    public string? NovaSenha { get; set; } = string.Empty;
}
