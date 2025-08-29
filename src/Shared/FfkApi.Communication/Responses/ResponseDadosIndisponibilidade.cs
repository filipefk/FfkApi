namespace FfkApi.Communication.Responses;

public class ResponseDadosIndisponibilidade
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string DataInicial { get; set; } = string.Empty;
    public string DataFinal { get; set; } = string.Empty;
    public ResponseDadosBasicosUsuario Usuario { get; set; } = new();
}
