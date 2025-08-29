namespace FfkApi.API.Middleware;

public class CabecalhosSegurancaMiddleware
{
    private readonly RequestDelegate _next;

    public CabecalhosSegurancaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains"; // Força o uso de HTTPS.
        headers.XContentTypeOptions = "nosniff"; // Evita que o browser tente inferir o tipo de conteúdo (prevent sniffing).
        headers.XFrameOptions = "DENY"; // Impede que o site seja exibido em iframes (clickjacking).
        headers.XXSSProtection = "1; mode=block"; // Ativa o filtro contra XSS no navegador.

        await _next(context);
    }
}
