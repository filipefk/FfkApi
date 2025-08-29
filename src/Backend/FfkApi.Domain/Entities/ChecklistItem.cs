using FfkApi.Domain.Enums;

namespace FfkApi.Domain.Entities;

public class ChecklistItem : EntityBase
{
    public int Ordem { get; set; } = 0;
    public string Descricao { get; set; } = string.Empty;
    public TipoChecklistItem TipoChecklistItem { get; set; } = TipoChecklistItem.Indefinido;
    public ICollection<ChecklistRespostaPossivel> ChecklistRespostasPossiveis { get; set; } = [];

    public Guid? IdDependeDeChecklistItem { get; set; } = null;
    public ChecklistItem? DependeDeChecklistItem { get; set; } = null;

    public ICollection<ChecklistGatilhoRespostaPossivel> GatilhosDeResposta { get; set; } = [];

    public Guid IdChecklist { get; set; }
    public Checklist Checklist { get; set; } = default!;

    public static ChecklistItem ItemSimples(string descricao, int ordem, string respostaInconformidade = "Não")
    {
        var checklistItem = new ChecklistItem()
        {
            Ordem = ordem,
            Descricao = descricao,
            TipoChecklistItem = TipoChecklistItem.Simples,
            ChecklistRespostasPossiveis =
            [
                ChecklistRespostaPossivel.Resposta("Sim", 1, respostaInconformidade.Contains("Sim", StringComparison.CurrentCultureIgnoreCase)),
                ChecklistRespostaPossivel.Resposta("Não", 2, respostaInconformidade.Contains("Não", StringComparison.CurrentCultureIgnoreCase)),
                ChecklistRespostaPossivel.Resposta("N/A", 3, respostaInconformidade.Contains("N/A", StringComparison.CurrentCultureIgnoreCase))
            ],
            GatilhosDeResposta = null!,
        };

        return checklistItem;
    }

}


