namespace FfkApi.Domain.Entities;

public class ChecklistRespostaPossivel : EntityBase
{
    public int Ordem { get; set; } = 0;
    public string Descricao { get; set; } = string.Empty;
    public bool GeraInconformidade { get; set; } = false;
    public Guid IdChecklistItem { get; set; }
    public ChecklistItem ChecklistItem { get; set; } = default!;

    public static ChecklistRespostaPossivel Resposta(string descricao, int ordem, bool geraInconformidade = false)
    {
        return new()
        {
            Ordem = ordem,
            Descricao = descricao,
            GeraInconformidade = geraInconformidade
        };
    }
}
