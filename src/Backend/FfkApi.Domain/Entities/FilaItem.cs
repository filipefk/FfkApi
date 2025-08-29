namespace FfkApi.Domain.Entities;

public class FilaItem : EntityBase
{
    public Guid IdFila { get; set; }
    public Fila Fila { get; set; } = default!;
    public long Posicao { get; set; } = 0;
}
