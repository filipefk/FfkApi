namespace FfkApi.Communication.Responses;

public class ResponseArquivoAnexo
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string? MimeType { get; set; } = string.Empty;
    public Stream stream { get; set; } = null!;
}
