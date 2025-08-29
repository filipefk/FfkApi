using GeradorDeCodigo.Controle;
using System.Reflection;
using System.Text.Json;

namespace GeradorDeCodigo.Util;

public class ArquivoUtil
{
    public static List<string> ListarArquivosDaPasta(string caminho)
    {
        if (string.IsNullOrWhiteSpace(caminho))
            throw new ArgumentException("O caminho da pasta não pode ser nulo ou vazio.", nameof(caminho));

        if (!Directory.Exists(caminho))
            throw new DirectoryNotFoundException($"A pasta '{caminho}' não foi encontrada.");

        return [.. Directory.GetFiles(caminho, "*", SearchOption.AllDirectories)];
    }

    public static ControleCodigo LerControleCodigoArquivo(string caminhoArquivo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentException("O caminho do arquivo não pode ser nulo ou vazio.", nameof(caminhoArquivo));

        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"O arquivo '{caminhoArquivo}' não foi encontrado.");

        string conteudo = File.ReadAllText(caminhoArquivo);

        const string inicio = "<#";
        const string fim = "#>";

        int idxInicio = conteudo.IndexOf(inicio, StringComparison.Ordinal);
        int idxFim = conteudo.IndexOf(fim, StringComparison.Ordinal);

        if (idxInicio == -1 || idxFim == -1 || idxFim < idxInicio)
            throw new InvalidOperationException("O arquivo não contém um bloco JSON válido entre <# e #>.");

        int jsonStart = idxInicio + inicio.Length;
        int jsonLength = idxFim - jsonStart;
        string json = conteudo.Substring(jsonStart, jsonLength).Trim();

        ControleCodigo controle = JsonSerializer.Deserialize<ControleCodigo>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        ) ?? throw new InvalidOperationException("Falha ao desserializar o JSON para ControleCodigo.");

        string restante = conteudo[(idxFim + fim.Length)..];

        while (restante[..1] == "\r" || restante[..1] == "\n")
        {
            restante = restante[1..];
        }

        controle.ConteudoArquivo = restante;

        return controle;
    }

    public static string? CriarArquivoComDiretorio(string caminhoArquivo, string conteudo)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentException("O caminho do arquivo não pode ser nulo ou vazio.", nameof(caminhoArquivo));

        if (File.Exists(caminhoArquivo))
        {
            Console.WriteLine($"O arquivo {caminhoArquivo} já existia e não foi alterado.");
            return null;
        }

        string? diretorio = Path.GetDirectoryName(caminhoArquivo);

        if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
        {
            Directory.CreateDirectory(diretorio);
        }

        File.WriteAllText(caminhoArquivo, conteudo);

        return caminhoArquivo;
    }

    public static bool AdicionouBlocoDeCodigoNaFuncao(
        string caminhoArquivo,
        string assinaturaFuncao,
        string blocoDeCodigoParaAdicionar,
        bool ignorarSeJaExistir = false,
        string? adicionarAntesDaLinha = null)
    {
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
            throw new ArgumentException("O caminho do arquivo não pode ser nulo ou vazio.", nameof(caminhoArquivo));

        if (string.IsNullOrWhiteSpace(assinaturaFuncao))
            throw new ArgumentException("A assinatura da função não pode ser nula ou vazia.", nameof(assinaturaFuncao));

        if (string.IsNullOrWhiteSpace(blocoDeCodigoParaAdicionar))
            throw new ArgumentException("O bloco de código para adicionar não pode ser nulo ou vazio.", nameof(blocoDeCodigoParaAdicionar));

        var encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var linhas = File.ReadAllLines(caminhoArquivo, encoding);
        int indiceAssinatura = -1;
        string assinaturaTrim = assinaturaFuncao.Trim();

        for (int i = 0; i < linhas.Length; i++)
        {
            if (linhas[i].Contains(assinaturaTrim, StringComparison.Ordinal))
            {
                indiceAssinatura = i;
                break;
            }
        }

        if (indiceAssinatura == -1)
            throw new Exception("Assinatura da função não encontrada.");

        var linhasBloco = blocoDeCodigoParaAdicionar.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        string primeiraLinhaTrim = linhasBloco[0].Trim();

        int nivelChave = 0;
        int indiceAdicionar = -1;
        for (int i = indiceAssinatura + 1; i < linhas.Length; i++)
        {
            if (linhas[i].Contains('{'))
                nivelChave++;
            if (linhas[i].Contains('}'))
                nivelChave--;

            if (ignorarSeJaExistir && linhas[i].Contains(primeiraLinhaTrim, StringComparison.Ordinal))
            {
                Console.WriteLine($"A primeira linha do bloco de código '{primeiraLinhaTrim}' já existe na função '{assinaturaFuncao}' do arquivo '{caminhoArquivo}' e o bloco não foi adicionado novamente.");
                return false;
            }

            if (adicionarAntesDaLinha != null && linhas[i].Contains(adicionarAntesDaLinha, StringComparison.Ordinal))
            {
                indiceAdicionar = i;
                break;
            }

            if (nivelChave == 0)
            {
                indiceAdicionar = i;
                break;
            }
        }

        if (indiceAdicionar == -1)
            throw new Exception("Não foi possível encontrar o final da função.");

        var linhasList = new List<string>(linhas);

        for (int j = linhasBloco.Length - 1; j >= 0; j--)
        {
            linhasList.Insert(indiceAdicionar, linhasBloco[j]);
        }

        File.WriteAllLines(caminhoArquivo, linhasList, encoding);

        return true;
    }

    public static Dictionary<string, string> BuscaPastasSistema()
    {
        var pastas = new Dictionary<string, string>(StringComparer.Ordinal);

        string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        DirectoryInfo dirInfo = new(exeDir);

        for (int i = 0; i < 3 && dirInfo.Parent != null; i++)
            dirInfo = dirInfo.Parent;

        pastas["PastaModelos"] = @$"{dirInfo.FullName}\Modelos\Crud";

        for (int i = 0; i < 2 && dirInfo.Parent != null; i++)
            dirInfo = dirInfo.Parent;

        string pastaBackend = @$"{dirInfo.FullName}\src\Backend";
        string pastaShared = @$"{dirInfo.FullName}\src\Shared";
        pastas["PastaBackend"] = pastaBackend;
        pastas["PastaShared"] = pastaShared;
        pastas["PastaTestes"] = @$"{dirInfo.FullName}\tests";

        pastas["PastaApi"] = @$"{pastaBackend}\FfkApi.API";
        pastas["PastaApplication"] = @$"{pastaBackend}\FfkApi.Application";
        pastas["PastaDomain"] = @$"{pastaBackend}\FfkApi.Domain";
        pastas["PastaInfrastructure"] = @$"{pastaBackend}\FfkApi.Infrastructure";
        pastas["PastaCommunication"] = @$"{pastaShared}\FfkApi.Communication";

        return pastas;
    }
}