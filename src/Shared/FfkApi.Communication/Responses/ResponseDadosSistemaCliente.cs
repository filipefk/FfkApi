namespace FfkApi.Communication.Responses;

public class ResponseDadosSistemaCliente
{
    public Guid Id { get; set; }
    public Guid AppId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
