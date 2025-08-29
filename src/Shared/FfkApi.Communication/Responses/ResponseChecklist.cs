namespace FfkApi.Communication.Responses;

public class ResponseChecklist
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public IList<ResponseChecklistItem> ChecklistItens { get; set; } = [];
}
