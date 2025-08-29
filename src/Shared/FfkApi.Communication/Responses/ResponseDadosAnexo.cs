namespace FfkApi.Communication.Responses;

public class ResponseDadosAnexo
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; } = 0L;
}
