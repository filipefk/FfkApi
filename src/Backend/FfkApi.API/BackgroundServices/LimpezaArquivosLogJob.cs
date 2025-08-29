using FfkApi.Application.UseCases.Limpeza.LimpezaLogs;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FfkApi.API.BackgroundServices;

public class LimpezaArquivosLogJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly uint _limpezaArquivosLogDias;

    public LimpezaArquivosLogJob(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _limpezaArquivosLogDias = configuration
            .GetSection("Configuracoes:Limpeza:LimpezaArquivos:LimpezaArquivosLogDias")
            .Get<uint>();
    }

    public async Task ExecutarAsync([FromServices] PerformContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var limpezaLogsAntigos = scope.ServiceProvider.GetRequiredService<ILimpezaArquivosLogUseCase>();

        LogaMensagem($"Procurando arquivos de log com {_limpezaArquivosLogDias} dias ou mais para limpeza", context);

        var arquivos = await limpezaLogsAntigos.Execute(CancellationToken.None);

        foreach (var arquivo in arquivos)
        {
            LogaMensagem($"Removido arquivo {arquivo}", context);
        }

        LogaMensagem($"Removidos {arquivos.Count} arquivos de log", context);
    }

    private void LogaMensagem(string mensagem, PerformContext context)
    {
        Log.Information($"{this.GetType().Name} - {mensagem}");
        context.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {mensagem}");
    }
}
