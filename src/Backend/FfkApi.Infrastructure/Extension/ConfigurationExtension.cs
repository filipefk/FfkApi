using Microsoft.Extensions.Configuration;

namespace FfkApi.Infrastructure.Extension;

public static class ConfigurationExtension
{
    public static bool RodandoTesteEmMemoria(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>("TesteEmMemoria");
    }

    public static bool RodandoTesteAceitacao(this IConfiguration _)
    {
        return Environment.GetEnvironmentVariable("RODANDO_TESTE_ACEITACAO") == "true";
    }
}
