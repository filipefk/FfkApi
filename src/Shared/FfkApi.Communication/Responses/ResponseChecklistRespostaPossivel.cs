namespace FfkApi.Communication.Responses;

public class ResponseChecklistRespostaPossivel
{
    public int Ordem { get; set; } = 0;
    public string Descricao { get; set; } = string.Empty;
    public bool GeraInconformidade { get; set; } = false;
}
