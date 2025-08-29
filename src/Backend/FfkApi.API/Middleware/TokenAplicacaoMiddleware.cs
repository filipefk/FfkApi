using FfkApi.Communication.Responses;
using FfkApi.Exceptions;

namespace FfkApi.API.Middleware;

public class TokenAplicacaoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TokenAplicacaoMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/jobs", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("x-app-token", out var token))
        {
            await EscreverErroAsync(context, ResourceMessagesException.TOKEN_APLICACAO_AUSENTE);
            return;
        }

        if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
        {
            var tokenHealthCheck = _configuration
                .GetSection("Configuracoes:TokensAplicacao:TokenHealthCheck")
                .Get<string>();
            if (tokenHealthCheck == null || tokenHealthCheck != token.ToString())
            {
                await EscreverErroAsync(context, ResourceMessagesException.TOKEN_APLICACAO_INVALIDO);
                return;
            }
            await _next(context);
            return;
        }

        var tokensPermitidos = _configuration
            .GetSection("Configuracoes:TokensAplicacao:TokensPermitidos")
            .Get<string[]>();

        if (tokensPermitidos == null || !tokensPermitidos.ToList().Contains(token.ToString()))
        {
            await EscreverErroAsync(context, ResourceMessagesException.TOKEN_APLICACAO_INVALIDO);
            return;
        }

        await _next(context);
    }

    private static async Task EscreverErroAsync(HttpContext context, string mensagem)
    {
        var erro = new ResponseErro(mensagem)
        {
            TokenEstaExpirado = false
        };

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(erro);
    }
}