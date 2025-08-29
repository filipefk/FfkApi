namespace FfkApi.Domain.Configurations;

public static class ConfiguracaoArquivoAnexo
{
    public static long TamanhoMaximoBytes { get; private set; } = 0;
    public static string TamanhoMaximoBytesTexto { get; private set; } = string.Empty;

    public static void Inicializar(long tamanhoMaximo)
    {
        TamanhoMaximoBytes = tamanhoMaximo;

        string[] sufixos = { "B", "KB", "MB", "GB", "TB" };
        int ordem = 0;
        double tamanhoFormatado = tamanhoMaximo;

        while (tamanhoFormatado >= 1024 && ordem < sufixos.Length - 1)
        {
            ordem++;
            tamanhoFormatado /= 1024;
        }

        TamanhoMaximoBytesTexto = $"{tamanhoFormatado:0.#} {sufixos[ordem]}";
    }
}
