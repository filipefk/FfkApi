namespace FfkApi.Domain.Services.Email;

public class RespostaEnvioEmail
{
    public bool Enviado { get; set; } = false;
    public string Mensagem { get; set; } = string.Empty;
}
