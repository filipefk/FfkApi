using GeradorDeCodigo.Controle;

namespace GeradorDeCodigo.Util;

public class Substituidor
{
    public static List<VariavelCodigo> PrepararSubstituicoes(List<VariavelCodigo> variaveis, Dictionary<string, string> dicionario)
    {
        if (variaveis.Count == 0 || dicionario.Count == 0)
            return variaveis;

        foreach (var variavel in variaveis)
        {
            var chave = variavel.SubstituirPor;
            if (chave != null && dicionario.TryGetValue(chave, out var novoValor))
            {
                var modificador = variavel.Modificador;
                if (!string.IsNullOrEmpty(modificador))
                {
                    if (modificador == "ToUpper")
                        novoValor = novoValor.ToUpperInvariant();
                    else if (modificador == "ToLower")
                        novoValor = novoValor.ToLowerInvariant();
                    else if (modificador == "Variavel")
                        novoValor = char.ToLowerInvariant(novoValor[0]) + novoValor.Substring(1);
                }
                variavel.SubstituirPor = novoValor;
            }
        }
        return variaveis;
    }

    public static string SubstituirVariaveis(string texto, List<VariavelCodigo> variaveis)
    {
        if (string.IsNullOrEmpty(texto) || variaveis == null || variaveis.Count == 0)
            return texto;

        foreach (var variavel in variaveis)
        {
            if (!string.IsNullOrEmpty(variavel.Nome))
            {
                texto = texto.Replace(variavel.Nome, variavel.SubstituirPor);
            }
        }
        return texto;
    }
}
