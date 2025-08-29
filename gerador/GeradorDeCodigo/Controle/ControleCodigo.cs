namespace GeradorDeCodigo.Controle;

public class ControleCodigo
{
    public List<VariavelCodigo> Variaveis { get; set; } = [];
    public string ArquivoDestino { get; set; } = string.Empty;
    public List<InclusaoCodigo>? InclusoesCodigo { get; set; } = [];
    public string ConteudoArquivo { get; set; } = string.Empty;
}
