using FfkApi.Domain.Services.Mensageria;
using FfkApi.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FfkApi.API.HealthCheck;

public class PublicarMensagemServiceHealthCheck : IHealthCheck
{
    private readonly IPublicarMensagemService _publicarMensagemService;

    public PublicarMensagemServiceHealthCheck(IPublicarMensagemService publicarMensagemService)
    {
        _publicarMensagemService = publicarMensagemService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var disponivel = _publicarMensagemService.EstaDisponivel();

        return Task.FromResult(
            disponivel
                ? HealthCheckResult.Healthy(ResourceMessagesException.MENSAGERIA_DISPONIVEL)
                : HealthCheckResult.Unhealthy(ResourceMessagesException.MENSAGERIA_INDISPONIVEL)
        );
    }
}
