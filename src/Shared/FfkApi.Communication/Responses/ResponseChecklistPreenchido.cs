namespace FfkApi.Communication.Responses;

public class ResponseChecklistPreenchido
{
    public string NomeChecklist { get; set; } = string.Empty;
    public string DescricaoChecklist { get; set; } = string.Empty;
    public IList<ResponseChecklistPreenchidoItem> ChecklistPreenchidoItens { get; set; } = [];
}
