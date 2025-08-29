using Microsoft.Extensions.Configuration;

namespace FfkApi.Application.UseCases.Limpeza.LimpezaLogs;

public class LimpezaArquivosLogUseCase : ILimpezaArquivosLogUseCase
{
    private readonly string _logDirectory;
    private readonly uint _limpezaArquivosLogDias;

    public LimpezaArquivosLogUseCase(IConfiguration configuration)
    {
        _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        _limpezaArquivosLogDias = configuration
            .GetSection("Configuracoes:Limpeza:LimpezaArquivos:LimpezaArquivosLogDias")
            .Get<uint>();
    }

    public Task<List<string>> Execute(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_logDirectory))
            return Task.FromResult(new List<string>());

        var arquivos = Directory.GetFiles(_logDirectory, "log*.txt");

        List<string> excluir = [];
        List<string> excluidos = [];

        var dataLimite = DateTime.Today.AddDays(-_limpezaArquivosLogDias);

        foreach (var arquivo in arquivos)
        {
            var nomeArquivo = Path.GetFileNameWithoutExtension(arquivo);
            if (nomeArquivo.Length != 11)
                continue;

            var dataStr = nomeArquivo.Substring(3, 8);
            if (DateTime.TryParseExact(dataStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dataArquivo))
            {
                if (dataArquivo <= dataLimite)
                {
                    excluir.Add(arquivo);
                    excluidos.Add(arquivo);
                }
            }
        }

        foreach (var arquivo in excluir)
        {
            try { File.Delete(arquivo); } catch { excluidos.Remove(arquivo); }
        }

        return Task.FromResult(excluidos);
    }
}
