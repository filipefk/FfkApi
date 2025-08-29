namespace FfkApi.Domain.Entities;

public class ChecklistPreenchido : EntityBase
{
    public string NomeChecklist { get; set; } = string.Empty;
    public string DescricaoChecklist { get; set; } = string.Empty;
    public ICollection<ChecklistPreenchidoItem> ChecklistPreenchidoItens { get; set; } = [];
    public Guid IdChecklist { get; set; }
    public Checklist Checklist { get; set; } = default!;
}

