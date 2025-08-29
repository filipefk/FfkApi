using FfkApi.Application.UseCases.EnvioEmail.EnvioEmailAtivacao;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FfkApi.API.BackgroundServices;

public class EnviarEmailAtivacaoUsuarioJob
{
    private readonly IServiceProvider _serviceProvider;

    public EnviarEmailAtivacaoUsuarioJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecutarAsync([FromServices] PerformContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var envioEmailAtivacaoUseCase = scope.ServiceProvider.GetRequiredService<IEnvioEmailAtivacaoUseCase>();

        LogaMensagem("Procurando usuários para envio de e-mail de ativação", context);

        var usuarios = await envioEmailAtivacaoUseCase.Execute(CancellationToken.None);

        if (usuarios is null || usuarios.Count == 0)
        {
            LogaMensagem("Nenhum usuário para envio de e-mail de ativação", context);
            return;
        }

        foreach (var usuario in usuarios)
        {
            LogaMensagem($"Enviado e-mail de ativação para {usuario.Email} Id = {usuario.Id}", context);
        }
        context.WriteLine($"Enviados {usuarios.Count} e-mails de ativação");
    }

    private void LogaMensagem(string mensagem, PerformContext context)
    {
        Log.Information($"{this.GetType().Name} - {mensagem}");
        context.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {mensagem}");
    }
}

