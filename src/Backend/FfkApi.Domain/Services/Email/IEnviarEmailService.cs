namespace FfkApi.Domain.Services.Email;

public interface IEnviarEmailService
{
    Task<RespostaEnvioEmail> EnviarEmailAsync(
        string? remetenteEmail,
        string? remetenteNome,
        string destinatarioEmail,
        string destinatarioNome,
        string assunto,
        string? modeloEmail,
        string? textoEmail,
        Dictionary<string, string> variaveis,
        CancellationToken cancellationToken);

    bool EstaDisponivel();
}
