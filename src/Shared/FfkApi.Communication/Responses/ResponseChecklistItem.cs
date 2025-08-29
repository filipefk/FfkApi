namespace FfkApi.Communication.Responses;

public class ResponseChecklistItem
{
    public int Ordem { get; set; } = 0;
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public IList<ResponseChecklistRespostaPossivel> ChecklistRespostasPossiveis { get; set; } = [];
    public string? DependeDeChecklistItem { get; set; } = null!;
    public IList<string>? GatilhosDeResposta { get; set; } = null!;
}
