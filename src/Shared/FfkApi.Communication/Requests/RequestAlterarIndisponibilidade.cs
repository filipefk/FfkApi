namespace FfkApi.Communication.Requests;

public class RequestAlterarIndisponibilidade
{
    public string? Id { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string? DataInicial { get; set; } = string.Empty;
    public string? DataFinal { get; set; } = string.Empty;
    public string? Usuario { get; set; } = string.Empty;
}
