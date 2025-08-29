namespace FfkApi.Communication.Responses;

public class ResponseMembroEquipe
{
    public Guid Id { get; set; }
    public Guid IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Lider { get; set; } = false;
}
