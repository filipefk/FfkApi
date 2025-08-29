namespace FfkApi.Communication.Requests;

public class RequestAdicionarAnexoFeed : RequestCadastrarAnexo
{
    public string? Id { get; set; } = string.Empty;
}
