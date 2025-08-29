namespace GeradorDeCodigo.Controle;

public class InclusaoCodigo
{
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string AssinaturaFuncao { get; set; } = string.Empty;
    public string BlocoAdicionar { get; set; } = string.Empty;
    public bool IgnorarSeJaExistir { get; set; } = false;
    public string? AdicionarAntesDaLinha { get; set; } = null;
}
