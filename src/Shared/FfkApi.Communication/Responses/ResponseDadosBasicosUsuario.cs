namespace FfkApi.Communication.Responses;

public class ResponseDadosBasicosUsuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Organizacao { get; set; } = string.Empty;
}
