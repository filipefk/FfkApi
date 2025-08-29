namespace FfkApi.Communication.Responses;

public class ResponseDadosFeed
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? PalavrasChave { get; set; } = null;
    public string Status { get; set; } = string.Empty;
    public IList<ResponseDadosAnexo> Anexos { get; set; } = [];
    public IList<string> VisibilidadeUsuarios { get; set; } = [];
    public IList<string> VisibilidadeEquipes { get; set; } = [];
    public string? ExpiraEm { get; set; } = null;
    public string Organizacao { get; set; } = string.Empty;
}
