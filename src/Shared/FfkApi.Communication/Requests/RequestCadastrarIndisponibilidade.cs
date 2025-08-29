namespace FfkApi.Communication.Requests;

public class RequestCadastrarIndisponibilidade
{
    public string? Descricao { get; set; } = string.Empty;
    public string? DataInicial { get; set; } = string.Empty;
    public string? DataFinal { get; set; } = string.Empty;
    public string? Usuario { get; set; } = string.Empty;
}