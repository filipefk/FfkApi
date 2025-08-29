using GeradorDeCodigo.Controle;
using GeradorDeCodigo.Util;

namespace GeradorDeCodigo.Geradores;

public class GeradorCrud
{
    private readonly Dictionary<string, string> pastas;

    public GeradorCrud()
    {
        pastas = ArquivoUtil.BuscaPastasSistema();
    }

    private static void RedirecionarConsoleParaArquivo(string caminhoArquivo)
    {
        var fileStream = new FileStream(caminhoArquivo, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
        var multiWriter = new MultiTextWriter(Console.Out, streamWriter);
        Console.SetOut(multiWriter);
        Console.SetError(multiWriter);
    }

    public void GerarCrud()
    {
        RedirecionarConsoleParaArquivo($"LogGeradorCrud_{DateTime.Now:yyyyMMddHHmmss}.log");

        var blocosDeCodigoAdicionados = new Dictionary<string, int>();
        var inclusoesCodigo = new List<InclusaoCodigo>();
        var arquivosCriados = new List<string?>();
        var controlesCodigo = new List<ControleCodigo>();
        var dicionario = new Dictionary<string, string>();

        var lstarArquivos = ArquivoUtil.ListarArquivosDaPasta(pastas["PastaModelos"]);

        Console.WriteLine("=== Carregando modelos ===\n");
        foreach (var arquivo in lstarArquivos)
        {
            var controleCodigo = ArquivoUtil.LerControleCodigoArquivo(arquivo);
            controlesCodigo.Add(controleCodigo);
        }

        Console.WriteLine("\n=== Obtendo valores para variáveis ===\n");
        foreach (var controleCodigo in controlesCodigo)
        {
            dicionario = PreencheValoresDasPastas(controleCodigo.Variaveis, dicionario);
            dicionario = ObterValoresDasVariaveis(controleCodigo.Variaveis, dicionario);
        }

        Console.WriteLine("\n=== Criando código ===\n");
        foreach (var controleCodigo in controlesCodigo)
        {
            controleCodigo.Variaveis = Substituidor.PrepararSubstituicoes(controleCodigo.Variaveis, dicionario);

            controleCodigo.ArquivoDestino = Substituidor.SubstituirVariaveis(controleCodigo.ArquivoDestino, controleCodigo.Variaveis);

            if (controleCodigo.InclusoesCodigo != null)
            {
                foreach (var novaInclusao in controleCodigo.InclusoesCodigo)
                {
                    novaInclusao.CaminhoArquivo = Substituidor.SubstituirVariaveis(novaInclusao.CaminhoArquivo, controleCodigo.Variaveis);
                    novaInclusao.AssinaturaFuncao = Substituidor.SubstituirVariaveis(novaInclusao.AssinaturaFuncao, controleCodigo.Variaveis);
                    novaInclusao.BlocoAdicionar = Substituidor.SubstituirVariaveis(novaInclusao.BlocoAdicionar, controleCodigo.Variaveis);

                    if (!string.IsNullOrWhiteSpace(novaInclusao.AdicionarAntesDaLinha))
                        novaInclusao.AdicionarAntesDaLinha = Substituidor.SubstituirVariaveis(novaInclusao.AdicionarAntesDaLinha, controleCodigo.Variaveis);

                    var jaTemInclusaoIgual = false;
                    foreach (var inclusaoJaPreparada in inclusoesCodigo)
                    {
                        var primeiraLinhaBloco1 = inclusaoJaPreparada.BlocoAdicionar.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n')[0].Trim();
                        var primeiraLinhaBloco2 = novaInclusao.BlocoAdicionar.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n')[0].Trim();

                        if (inclusaoJaPreparada.CaminhoArquivo == novaInclusao.CaminhoArquivo &&
                            inclusaoJaPreparada.AssinaturaFuncao == novaInclusao.AssinaturaFuncao &&
                            primeiraLinhaBloco1 == primeiraLinhaBloco2 &&
                            (inclusaoJaPreparada.IgnorarSeJaExistir || novaInclusao.IgnorarSeJaExistir))
                        {
                            jaTemInclusaoIgual = true;
                            break;
                        }
                    }

                    if (!jaTemInclusaoIgual)
                        inclusoesCodigo.Add(novaInclusao);
                }
            }

            controleCodigo.ConteudoArquivo = Substituidor.SubstituirVariaveis(controleCodigo.ConteudoArquivo, controleCodigo.Variaveis);
            arquivosCriados.Add(ArquivoUtil.CriarArquivoComDiretorio(controleCodigo.ArquivoDestino, controleCodigo.ConteudoArquivo));
        }

        Console.WriteLine("\n=== Adicionando código em arquivos existentes ===\n");
        foreach (var inclusaoCodigo in inclusoesCodigo)
        {
            if (ArquivoUtil.AdicionouBlocoDeCodigoNaFuncao(
                inclusaoCodigo.CaminhoArquivo,
                inclusaoCodigo.AssinaturaFuncao,
                inclusaoCodigo.BlocoAdicionar,
                inclusaoCodigo.IgnorarSeJaExistir,
                inclusaoCodigo.AdicionarAntesDaLinha))
            {
                if (blocosDeCodigoAdicionados.TryGetValue(inclusaoCodigo.CaminhoArquivo, out int value))
                    blocosDeCodigoAdicionados[inclusaoCodigo.CaminhoArquivo] = ++value;
                else
                    blocosDeCodigoAdicionados[inclusaoCodigo.CaminhoArquivo] = 1;
            }
        }

        Console.WriteLine("\n=== Relatório final ===\n");

        RelatorioFinal(blocosDeCodigoAdicionados, arquivosCriados);

        Console.WriteLine("\nPressione qualquer tecla para encerrar...");
        Console.ReadKey();
    }

    private static void RelatorioFinal(Dictionary<string, int> blocosDeCodigoAdicionados, List<string?> arquivosCriados)
    {
        Console.WriteLine("Processo de geração de CRUD concluído!");
        if (blocosDeCodigoAdicionados.Count == 0)
        {
            Console.WriteLine("Nenhum bloco de código foi adicionada.");
        }
        else
        {
            foreach (var kvp in blocosDeCodigoAdicionados)
            {
                Console.WriteLine($"Adicionados {kvp.Value} blocos de código no arquivo {kvp.Key}");
            }
        }

        Console.WriteLine($"Criados {arquivosCriados.Count(a => a != null)} arquivos:");
        foreach (var arquivo in arquivosCriados.Where(a => a != null).ToList())
            Console.WriteLine(arquivo);
    }

    private Dictionary<string, string> PreencheValoresDasPastas(List<VariavelCodigo> variaveis, Dictionary<string, string> dicionario)
    {
        foreach (var variavel in variaveis)
        {
            var chave = variavel.SubstituirPor;
            if (chave.StartsWith("Pasta", StringComparison.OrdinalIgnoreCase) && !dicionario.ContainsKey(chave))
            {
                dicionario[chave] = pastas[chave];
            }
        }
        return dicionario;
    }

    private static Dictionary<string, string> ObterValoresDasVariaveis(List<VariavelCodigo> variaveis, Dictionary<string, string> dicionario)
    {
        foreach (var variavel in variaveis)
        {
            var chave = variavel.SubstituirPor;
            if (!dicionario.ContainsKey(chave))
            {
                Console.Write($"Informe o valor para {chave}: ");
                var valor = Console.ReadLine() ?? string.Empty;
                Console.WriteLine($"{chave} = {valor}");
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Console.WriteLine("Processo cancelado!");
                    Console.WriteLine("Pressione qualquer tecla para fechar o console...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                dicionario[chave] = valor;
            }
        }
        return dicionario;
    }
}