using FfkApi.Domain.Services.Auditoria;
using FfkApi.Domain.Services.UsuarioLogado;
using System.Net;
using System.Text.RegularExpressions;

namespace FfkApi.API.Middleware;

public class EventosSegurancaMiddleware
{
    private readonly RequestDelegate _next;

    public EventosSegurancaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var auditService = context.RequestServices.GetService<IAuditoriaSegurancaService>();
        if (auditService == null)
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        string? corpoRequisicao = await LerCorpoRequisicaoAsync(context);

        var corpoOriginal = context.Response.Body;

        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        using var reader = new StreamReader(memStream);

        if (EventoSeguranca(context.Response.StatusCode))
        {
            memStream.Seek(0, SeekOrigin.Begin);

            var corpoResposta = await reader.ReadToEndAsync();

            var nomeUsuario = await PewgarNomeUsuarioAsync(context);
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var evento = NomeEvento(context.Response.StatusCode);
            var detalhes = MontarDetalhes(corpoRequisicao, corpoResposta);

            await auditService.LogAsync(
                evento: evento,
                usuario: nomeUsuario,
                caminho: context.Request.Path,
                metodo: context.Request.Method,
                ip: ip,
                detalhes: detalhes
            );
        }

        memStream.Seek(0, SeekOrigin.Begin);
        await memStream.CopyToAsync(corpoOriginal);
        context.Response.Body = corpoOriginal;
    }

    private static async Task<string?> LerCorpoRequisicaoAsync(HttpContext context)
    {
        if (context.Request.ContentLength > 0)
        {
            // Verifica se é multipart/form-data
            if (context.Request.ContentType != null &&
                context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                // Lê apenas os campos não-arquivo
                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();
                    var dict = new Dictionary<string, string>();
                    foreach (var field in form)
                    {
                        dict[field.Key] = field.Value!;
                    }
                    // Não inclui arquivos (form.Files)
                    context.Request.Body.Position = 0;
                    return System.Text.Json.JsonSerializer.Serialize(dict);
                }
                context.Request.Body.Position = 0;
                return null;
            }
            else
            {
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var corpoRequisicao = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                return corpoRequisicao;
            }
        }
        return null;
    }

    private static bool EventoSeguranca(int statusCode) =>
        statusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden or (int)HttpStatusCode.TooManyRequests;

    private static string NomeEvento(int statusCode) =>
        statusCode switch
        {
            401 => "401 Unauthorized",
            403 => "403 Forbidden",
            429 => "429 TooManyRequests",
            _ => "Evento desconhecido"
        };

    private static async Task<string?> PewgarNomeUsuarioAsync(HttpContext context)
    {
        try
        {
            var usuarioLogado = context.RequestServices.GetService<IUsuarioLogadoService>();
            if (usuarioLogado != null)
            {
                var usuario = await usuarioLogado.PegarUsuarioDoTokenEnviado(CancellationToken.None);
                return usuario?.Nome;
            }
        }
        catch { }
        return null;
    }

    private static string? MontarDetalhes(string? corpoRequisicao, string? corpoResposta)
    {
        corpoRequisicao = MascararSenha(corpoRequisicao);
        if (!string.IsNullOrWhiteSpace(corpoRequisicao) && !string.IsNullOrWhiteSpace(corpoResposta))
        {
            return $"Request: {Regex.Unescape(corpoRequisicao)} Response: {Regex.Unescape(corpoResposta)}";
        }
        if (!string.IsNullOrWhiteSpace(corpoRequisicao))
        {
            return $"Request: {Regex.Unescape(corpoRequisicao)}";
        }
        if (!string.IsNullOrWhiteSpace(corpoResposta))
        {
            return $"Response: {Regex.Unescape(corpoResposta)}";
        }
        return null;
    }
    private static string? MascararSenha(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        return Regex.Replace(
            json,
            @"(""senha""\s*:\s*)"".*?""",
            "$1\"*****\"",
            RegexOptions.IgnoreCase
        );
    }
}
