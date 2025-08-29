namespace FfkApi.Domain.Entities;

public class Checklist : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public ICollection<ChecklistItem> ChecklistItens { get; set; } = [];
    public Guid IdOrganizacao { get; set; } = default!;
    public Organizacao Organizacao { get; set; } = default!;
}
