namespace FfkApi.Communication.Requests;

public class RequestAlterarPermissoesUsuario
{
    public string? Id { get; set; } = string.Empty;
    public IList<string>? PerfisAcesso { get; set; } = null;
    public IList<string>? Permissoes { get; set; } = null;
}
