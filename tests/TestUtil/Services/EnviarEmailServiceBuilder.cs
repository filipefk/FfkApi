using FfkApi.Domain.Services.Email;
using Moq;

namespace TestUtil.Services;

public class EnviarEmailServiceBuilder
{
    private readonly Mock<IEnviarEmailService> _enviarEmailService;

    public EnviarEmailServiceBuilder()
    {
        _enviarEmailService = new Mock<IEnviarEmailService>();
    }

    public IEnviarEmailService Build()
    {
        return _enviarEmailService.Object;
    }

    public void SetupEnviarEmailAsyncReturnsRespostaEnvioEmail(RespostaEnvioEmail resposta)
    {
        _enviarEmailService
            .Setup(service => service.EnviarEmailAsync(
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(resposta);
    }
}

/*
 string? remetenteEmail,
        string? remetenteNome,
        string destinatarioEmail,
        string destinatarioNome,
        string assunto,
        string? modeloEmail,
        string? textoEmail,
        Dictionary<string, string> variaveis,
        CancellationToken cancellationToken);
 */
