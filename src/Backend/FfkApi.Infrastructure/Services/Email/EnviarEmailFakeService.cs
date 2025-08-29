using FfkApi.Domain.Services.Email;

namespace FfkApi.Infrastructure.Services.Email;

public class EnviarEmailFakeService : IEnviarEmailService
{
    public Task<RespostaEnvioEmail> EnviarEmailAsync(
        string? remetenteEmail,
        string? remetenteNome,
        string destinatarioEmail,
        string destinatarioNome,
        string assunto,
        string? modeloEmail,
        string? textoEmail,
        Dictionary<string, string> variaveis,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new RespostaEnvioEmail
        {
            Enviado = true,
            Mensagem = string.Empty
        });
    }

    public bool EstaDisponivel()
    {
        return true;
    }
}
