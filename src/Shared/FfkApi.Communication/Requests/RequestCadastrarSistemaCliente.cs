namespace FfkApi.Communication.Requests;

public class RequestCadastrarSistemaCliente
{
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;
    public string? Senha { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
}