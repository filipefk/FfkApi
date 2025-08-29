namespace FfkApi.Domain.Entities;

public class ChecklistPreenchidoItem : EntityBase
{
    public int OrdemItem { get; set; } = 0;
    public string DescricaoItem { get; set; } = string.Empty;
    public string DescricaoRespostaEscolhida { get; set; } = string.Empty;
    public bool GeraInconformidade { get; set; }
    public string? Observacao { get; set; }
    public Guid IdChecklistPreenchido { get; set; }
    public ChecklistPreenchido ChecklistPreenchido { get; set; } = default!;
}

