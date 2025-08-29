using FfkApi.Domain.Services.Arquivos;
using FfkApi.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FfkApi.API.HealthCheck;

public class ArmazenadorDeArquivoServiceHealthCheck : IHealthCheck
{
    private readonly IArmazenadorDeArquivoService _armazenadorDeArquivoService;

    public ArmazenadorDeArquivoServiceHealthCheck(IArmazenadorDeArquivoService armazenadorDeArquivoService)
    {
        _armazenadorDeArquivoService = armazenadorDeArquivoService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var disponivel = _armazenadorDeArquivoService.EstaDisponivel();

        return Task.FromResult(
            disponivel
                ? HealthCheckResult.Healthy(ResourceMessagesException.ARMAZENADOR_DE_ARQUIVOS_DISPONIVEL)
                : HealthCheckResult.Unhealthy(ResourceMessagesException.ARMAZENADOR_DE_ARQUIVOS_INDISPONIVEL)
        );
    }
}
