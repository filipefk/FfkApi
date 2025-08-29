namespace FfkApi.Domain.Configurations;

public static class ConfiguracaoFront
{
    public static string UrlFront { get; private set; } = string.Empty;

    public static void Inicializar(string urlFront)
    {
        UrlFront = urlFront;
    }
}
