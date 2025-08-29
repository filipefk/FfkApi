using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace FfkApi.API.HealthCheck;

public static class HealthChecksExtension
{
    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                connectionString: configuration.GetConnectionString("ConnectionPostgreSql")!,
                name: "PostgreSQL",
                timeout: TimeSpan.FromSeconds(5),
                tags: ["db", "postgresql", "pronto"])
            .AddCheck<PublicarMensagemServiceHealthCheck>(
                name: "Mensageria",
                tags: ["fila", "mensageria"])
            .AddCheck<EnviarEmailServiceHealthCheck>(
                name: "Email",
                tags: ["email"])
            .AddCheck<ArmazenadorDeArquivoServiceHealthCheck>(
                name: "Armazenador de arquivos",
                tags: ["arquivos", "pronto"]);
    }

    public static void MapHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health");

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("pronto"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        component = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        component = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });
    }
}
