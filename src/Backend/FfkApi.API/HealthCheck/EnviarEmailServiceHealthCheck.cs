using FfkApi.Domain.Services.Email;
using FfkApi.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FfkApi.API.HealthCheck;

public class EnviarEmailServiceHealthCheck : IHealthCheck
{
    private readonly IEnviarEmailService _enviarEmailService;

    public EnviarEmailServiceHealthCheck(IEnviarEmailService enviarEmailService)
    {
        _enviarEmailService = enviarEmailService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var disponivel = _enviarEmailService.EstaDisponivel();

        return Task.FromResult(
            disponivel
                ? HealthCheckResult.Healthy(ResourceMessagesException.ENVIAR_EMAIL_DISPONIVEL)
                : HealthCheckResult.Unhealthy(ResourceMessagesException.ENVIAR_EMAIL_INDISPONIVEL)
        );
    }
}
