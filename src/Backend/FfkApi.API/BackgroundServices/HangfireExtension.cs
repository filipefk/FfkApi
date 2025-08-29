using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;

namespace FfkApi.API.BackgroundServices;

public static class HangfireExtension
{
    private static bool _consoleInitialized = false;

    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionPostgreSql")!;
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(options =>
                  {
                      options.UseNpgsqlConnection(connectionString);
                  });

            if (!_consoleInitialized)
            {
                config.UseConsole();
                _consoleInitialized = true;
            }
        });

        services.AddHangfireServer();
    }

    public static void UseHangfire(this IApplicationBuilder app)
    {
        // TODO : Proteger o dashboard do Hangfire

        app.UseHangfireDashboard("/jobs");

        RecurringJob.AddOrUpdate<EnviarEmailAtivacaoUsuarioJob>(
            "enviar-email-ativacao-usuario",
            job => job.ExecutarAsync(null!),
            "0 * * * * *");

        RecurringJob.AddOrUpdate<EnviarEmailNovaSenhaUsuarioJob>(
            "enviar-email-nova-senha-usuario",
            job => job.ExecutarAsync(null!),
            "30 * * * * *");

        RecurringJob.AddOrUpdate<LimpezaAuditoriaSegurancaJob>(
            "limpeza-auditoria-seguranca",
            job => job.ExecutarAsync(null!),
            "0 1 * * *");

        RecurringJob.AddOrUpdate<LimpezaArquivosLogJob>(
            "limpeza-arquivos-log",
            job => job.ExecutarAsync(null!),
            "0 2 * * *");
    }
}
