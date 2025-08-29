namespace FfkApi.Communication.Requests;

public class RequestAlterarEquipe
{
    public string? Id { get; set; } = string.Empty;
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public IList<RequestMembroEquipe>? Membros { get; set; } = [];
    public string? Organizacao { get; set; } = string.Empty;
}
