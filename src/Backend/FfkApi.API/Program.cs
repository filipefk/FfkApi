using FfkApi.API.BackgroundServices;
using FfkApi.API.DebugUtil;
using FfkApi.API.Documentation;
using FfkApi.API.Filters;
using FfkApi.API.HealthCheck;
using FfkApi.API.Middleware;
using FfkApi.API.Security;
using FfkApi.API.Token;
using FfkApi.Application.Extension;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Extension;
using FfkApi.Initialization.Extension;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var escutarPorta = builder.Configuration.GetValue<int>("Configuracoes:EscutarPorta");
if (escutarPorta > 0)
{
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(escutarPorta);
    });
}

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new FfkApi.Api.Converters.StringConverter()));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerSecurity(builder.Configuration);

var strLogLevel = builder.Configuration.GetValue<string>("Logging:LogLevel:Default");
var enumLogEventLevel = Enum.TryParse<LogEventLevel>(strLogLevel, true, out var parsedLogEventLevel)
    ? parsedLogEventLevel
    : LogEventLevel.Information;
builder.Host.UseSerilog((context, configuration) =>
    configuration.WriteTo.File("logs/log.txt",
        enumLogEventLevel,
        rollingInterval: RollingInterval.Day));

builder.Services.AddMvc(option => option.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ITokenRecebido, TokenRecebido>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

if (!builder.Configuration.RodandoTesteEmMemoria())
{
    builder.Services.AddRateLimit(builder.Configuration);
    builder.Services.AddCorsPolicy(builder.Configuration, builder.Environment);
    builder.Services.AddHealthChecks(builder.Configuration);
    builder.Services.AddHangfire(builder.Configuration);
}

builder.Services.AddAssemblySwaggerExamples();

var app = builder.Build();

if (!builder.Configuration.RodandoTesteEmMemoria())
    app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseSwagger();
if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseMiddleware<CabecalhosSegurancaMiddleware>();
app.UseMiddleware<EventosSegurancaMiddleware>();
app.UseMiddleware<TokenAplicacaoMiddleware>();

app.UseAuthorization();

app.MapControllers();

if (!builder.Configuration.RodandoTesteEmMemoria())
{
    Log.Information("Aplicando migrations");
    app.Services.ApplyMigrations();
    Log.Information("Aplicando dados de inicialização");
    app.Services.ApplyDataInitialization();
    Log.Information("Configurando Rate Limiter");
    app.UseRateLimiter();
    Log.Information("Configurando Health Checks");
    app.MapHealthChecks();
    Log.Information("Configurando Jobs");
    app.UseHangfire();
}

if (builder.Environment.IsDevelopment() &&
    !builder.Configuration.RodandoTesteEmMemoria() &&
    !builder.Configuration.RodandoTesteAceitacao())
{
    DebugUtil.AbreSwaggerNoBrowser();
}

Log.Information("Inicializando API");
await app.RunAsync();

return;

public partial class Program
{
    protected Program() { }
}