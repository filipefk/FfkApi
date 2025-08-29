namespace FfkApi.API.Security;

public static class CorsPolicyExtension
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment() && configuration.GetValue<bool>("Configuracoes:Cors:LiberarTodosEmAmbienteDev"))
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy => policy.AllowAnyOrigin()
                                                                 .AllowAnyHeader()
                                                                 .AllowAnyMethod());
            });
            return;
        }

        var sectionKey = environment.IsDevelopment()
            ? "Configuracoes:Cors:OrigensPermitidasDev"
            : "Configuracoes:Cors:OrigensPermitidasProducao";

        var origensPermitidas = configuration.GetSection(sectionKey).Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy => policy.WithOrigins(origensPermitidas)
                                                             .AllowAnyHeader()
                                                             .AllowAnyMethod()
                                                             .AllowCredentials());
        });

    }
}
