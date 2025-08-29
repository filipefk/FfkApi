namespace FfkApi.Communication.Requests;

public class RequestAlterarSistemaCliente
{
    public string? Id { get; set; } = string.Empty;
    public string? AppId { get; set; } = string.Empty;
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string? Senha { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
}
