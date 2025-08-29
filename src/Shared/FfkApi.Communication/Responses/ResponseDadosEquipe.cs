namespace FfkApi.Communication.Responses;

public class ResponseDadosEquipe
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public IList<ResponseMembroEquipe> Membros { get; set; } = [];
    public string Organizacao { get; set; } = string.Empty;
}
