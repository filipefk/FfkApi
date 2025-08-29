namespace FfkApi.Domain.Entities;

public class Anexo : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string NomeArquivoArmazenamento { get; set; } = string.Empty;
    public string Extensao { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; } = 0L;
    public string? MimeType { get; set; } = string.Empty;
    public string? Texto { get; set; } = null;
}
