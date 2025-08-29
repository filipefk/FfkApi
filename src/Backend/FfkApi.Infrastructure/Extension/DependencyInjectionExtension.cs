using FfkApi.Domain.Security.Credenciais;
using FfkApi.Domain.Security.Criptografia;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Domain.Services.Acesso;
using FfkApi.Domain.Services.Arquivos;
using FfkApi.Domain.Services.Auditoria;
using FfkApi.Domain.Services.Email;
using FfkApi.Domain.Services.Fila;
using FfkApi.Domain.Services.Mensageria;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Infrastructure.DataAccess;
using FfkApi.Infrastructure.Security.Credenciais;
using FfkApi.Infrastructure.Security.Criptografia;
using FfkApi.Infrastructure.Security.Tokens;
using FfkApi.Infrastructure.Services.Acesso;
using FfkApi.Infrastructure.Services.Arquivos;
using FfkApi.Infrastructure.Services.Auditoria;
using FfkApi.Infrastructure.Services.Email;
using FfkApi.Infrastructure.Services.Fila;
using FfkApi.Infrastructure.Services.Mensageria;
using FfkApi.Infrastructure.Services.UsuarioLogado;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System.Reflection;

namespace FfkApi.Infrastructure.Extension;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
        AddPasswordEncryptor(services);
        AddTokens(services, configuration);
        AddServices(services, configuration);
        AddCredenciais(services);
        AddPolly(services, configuration);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionPostgreSql");
        services.AddDbContext<FfkApiDbContext>(options => { options.UseNpgsql(connectionString!); });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        AddScopedListaNamespace(services, "FfkApi.Infrastructure.DataAccess.Repositories");
    }

    private static void AddPasswordEncryptor(IServiceCollection services)
    {
        services.AddScoped<IEncriptadorSenha, EncriptadorBCrypt>();
    }

    private static void AddTokens(IServiceCollection services, IConfiguration configuration)
    {
        var tempoValidadeMinutosJwtUsuario = configuration.GetValue<uint>("Configuracoes:JwtUsuario:TempoValidadeMinutos");
        var chaveAssinaturaJwtUsuario = configuration.GetValue<string>("Configuracoes:JwtUsuario:ChaveAssinatura");
        services.AddScoped<IGeradorTokenUsuario>(_ => new GeradorTokenUsuario(tempoValidadeMinutosJwtUsuario, chaveAssinaturaJwtUsuario!));
        services.AddScoped<IValidadorTokenUsuario>(_ => new ValidadorTokenUsuario(chaveAssinaturaJwtUsuario!));

        var tempoValidadeMinutosJwtSistemaCliente = configuration.GetValue<uint>("Configuracoes:JwtSistemaCliente:TempoValidadeMinutos");
        var chaveAssinaturaJwtSistemaCliente = configuration.GetValue<string>("Configuracoes:JwtSistemaCliente:ChaveAssinatura");
        services.AddScoped<IGeradorTokenSistemaCliente>(_ => new GeradorTokenSistemaCliente(tempoValidadeMinutosJwtSistemaCliente, chaveAssinaturaJwtSistemaCliente!));
        services.AddScoped<IValidadorTokenSistemaCliente>(_ => new ValidadorTokenSistemaCliente(chaveAssinaturaJwtSistemaCliente!));

        var tempoValidadeDiasRefreshToken = configuration.GetValue<uint>("Configuracoes:RefreshToken:TempoValidadeDias");
        services.AddScoped<IGeradorRefreshToken>(_ => new GeradorRefreshToken(tempoValidadeDiasRefreshToken, new GeradorToken()));

        var tempoValidadeHorasTokenAtivacao = configuration.GetValue<uint>("Configuracoes:TokenAtivacao:TempoValidadeHoras");
        services.AddScoped<IGeradorTokenAtivacao>(_ => new GeradorTokenAtivacao(tempoValidadeHorasTokenAtivacao, new GeradorToken()));

        var tempoValidadeHorasTokenNovaSenha = configuration.GetValue<uint>("Configuracoes:TokenNovaSenha:TempoValidadeHoras");
        services.AddScoped<IGeradorTokenNovaSenha>(_ => new GeradorTokenNovaSenha(tempoValidadeHorasTokenNovaSenha, new GeradorToken()));
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUsuarioLogadoService, UsuarioLogadoService>();
        services.AddScoped<IFilaService, FilaService>();
        services.AddScoped<IAuditoriaSegurancaService, AuditoriaSegurancaService>();
        services.AddScoped<IArmazenadorDeArquivoService, ArmazenadorDeArquivoLocalService>();
        services.AddScoped<IAcessoService, AcessoService>();
        AddEnviarEmailService(services, configuration);
        AddPublicarMensagemService(services, configuration);
    }

    private static void AddEnviarEmailService(IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        services.AddScoped<IEnviarEmailService, EnviarEmailFakeService>();
#else
        var emailSendGridOptions = new EmailSendGridOptions();
        configuration.GetSection("Configuracoes:SendGrid").Bind(emailSendGridOptions);
        services.Configure<EmailSendGridOptions>(options =>
        {
            options.UrlApi = emailSendGridOptions.UrlApi;
            options.UrlApiQuota = emailSendGridOptions.UrlApiQuota;
            options.Token = emailSendGridOptions.Token;
            options.RemetenteEmailPadrao = emailSendGridOptions.RemetenteEmailPadrao;
            options.RemetenteNomePadrao = emailSendGridOptions.RemetenteNomePadrao;
        });

        services.AddScoped<IEnviarEmailService, EnviarEmailSendGridService>();
#endif
    }

    private static void AddPublicarMensagemService(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = new RabbitMqOptions();
        configuration.GetSection("Configuracoes:RabbitMq").Bind(rabbitMqOptions);
        services.Configure<RabbitMqOptions>(options =>
        {
            options.Host = rabbitMqOptions.Host;
            options.Porta = rabbitMqOptions.Porta;
            options.Usuario = rabbitMqOptions.Usuario;
            options.Senha = rabbitMqOptions.Senha;
            options.NomeFila = rabbitMqOptions.NomeFila;
        });
        services.AddSingleton<IPublicarMensagemService, RabbitMqPublicarService>();
    }

    private static IEnumerable<Type>? PegarClassesQueContemNamespace(string strNamespace)
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.Namespace != null && t.Namespace.Contains(strNamespace));
    }

    private static void AddScopedLista(IServiceCollection services, IEnumerable<Type>? lista)
    {
        if (lista == null)
            return;

        foreach (var type in lista)
        {
            var interfaceType = type.GetInterfaces().FirstOrDefault();
            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }

    private static void AddScopedListaNamespace(IServiceCollection services, string strNamespace)
    {
        AddScopedLista(services, PegarClassesQueContemNamespace(strNamespace));
    }

    private static void AddCredenciais(IServiceCollection services)
    {
        services.AddScoped<IGeradorSenhaValida, GeradorSenhaValida>();
        services.AddScoped<IGeradorToken, GeradorToken>();
    }

    private static void AddPolly(IServiceCollection services, IConfiguration configuration)
    {
        var tentativas = configuration.GetValue<int>("Configuracoes:Resiliencia:Polly:Tentativas");
        var tempoEsperaExponencialSegundos = configuration.GetValue<int>("Configuracoes:Resiliencia:Polly:TempoEsperaExponencialSegundos");
        var timeoutSegundos = configuration.GetValue<int>("Configuracoes:Resiliencia:Polly:TimeoutSegundos");

        services.AddSingleton<AsyncRetryPolicy>(
            Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(tentativas, retryAttempt => TimeSpan.FromSeconds(Math.Pow(tempoEsperaExponencialSegundos, retryAttempt)))
        );

        services.AddSingleton<AsyncRetryPolicy<HttpResponseMessage>>(
            Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(tentativas, retryAttempt => TimeSpan.FromSeconds(Math.Pow(tempoEsperaExponencialSegundos, retryAttempt)))
        );

        services.AddSingleton<AsyncTimeoutPolicy>(
            Policy.TimeoutAsync(TimeSpan.FromSeconds(timeoutSegundos))
        );
    }
}
