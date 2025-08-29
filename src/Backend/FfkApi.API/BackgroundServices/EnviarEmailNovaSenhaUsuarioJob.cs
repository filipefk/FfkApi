using FfkApi.Application.UseCases.EnvioEmail.EnvioEmailNovaSenha;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FfkApi.API.BackgroundServices;

public class EnviarEmailNovaSenhaUsuarioJob
{
    private readonly IServiceProvider _serviceProvider;

    public EnviarEmailNovaSenhaUsuarioJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecutarAsync([FromServices] PerformContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var envioEmailNovaSenhaUseCase = scope.ServiceProvider.GetRequiredService<IEnvioEmailNovaSenhaUseCase>();

        LogaMensagem("Procurando usuários para envio de e-mail de nova senha", context);

        var usuarios = await envioEmailNovaSenhaUseCase.Execute(CancellationToken.None);

        if (usuarios is null || usuarios.Count == 0)
        {
            LogaMensagem("Nenhum usuário para envio de e-mail de nova senha", context);
            return;
        }

        foreach (var usuario in usuarios)
        {
            LogaMensagem($"Enviado e-mail de nova senha para {usuario.Email} Id = {usuario.Id}", context);
        }
        context.WriteLine($"Enviados {usuarios.Count} e-mails de nova senha");
    }

    private void LogaMensagem(string mensagem, PerformContext context)
    {
        Log.Information($"{this.GetType().Name} - {mensagem}");
        context.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {mensagem}");
    }
}
