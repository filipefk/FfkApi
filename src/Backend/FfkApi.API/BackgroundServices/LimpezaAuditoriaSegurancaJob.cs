using FfkApi.Application.UseCases.Limpeza.LimpezaBanco;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FfkApi.API.BackgroundServices;

public class LimpezaAuditoriaSegurancaJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly uint _limpezaAuditoriaSegurancaDias;

    public LimpezaAuditoriaSegurancaJob(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _limpezaAuditoriaSegurancaDias = configuration
            .GetSection("Configuracoes:Limpeza:LimpezaBanco:LimpezaAuditoriaSegurancaDias")
            .Get<uint>();
    }

    public async Task ExecutarAsync([FromServices] PerformContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var limpezaAuditoriaSeguranca = scope.ServiceProvider.GetRequiredService<ILimpezaAuditoriaSegurancaUseCase>();

        LogaMensagem($"Procurando registros com {_limpezaAuditoriaSegurancaDias} dias ou mais para limpeza da tabela de auditoria de seguranca", context);

        var quant = await limpezaAuditoriaSeguranca.Execute(CancellationToken.None);

        LogaMensagem($"Removidos {quant} registros da tabela de auditoria de seguranca", context);
    }

    private void LogaMensagem(string mensagem, PerformContext context)
    {
        Log.Information($"{this.GetType().Name} - {mensagem}");
        context.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {mensagem}");
    }
}
