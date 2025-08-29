namespace FfkApi.Communication.Requests;

public class RequestTrocarSenha
{
    public string? SenhaAntiga { get; set; } = string.Empty;
    public string? NovaSenha { get; set; } = string.Empty;
}
