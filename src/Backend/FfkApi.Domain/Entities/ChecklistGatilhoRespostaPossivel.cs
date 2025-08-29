namespace FfkApi.Domain.Entities;

public class ChecklistGatilhoRespostaPossivel : EntityBase
{
    public Guid IdChecklistItem { get; set; }
    public ChecklistItem ChecklistItem { get; set; } = default!;

    public Guid IdChecklistRespostaPossivel { get; set; }
    public ChecklistRespostaPossivel RespostaGatilho { get; set; } = default!;
}
