using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace FfkApi.API.Filters;

public class SistemaClienteAutenticadoFilter : IAsyncAuthorizationFilter
{
    private readonly IValidadorTokenSistemaCliente _validadorTokenSistemaCliente;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;

    public SistemaClienteAutenticadoFilter(
        IValidadorTokenSistemaCliente validadorTokenSistemaCliente,
        ISistemaClienteRepository sistemaClienteRepository)
    {
        _validadorTokenSistemaCliente = validadorTokenSistemaCliente;
        _sistemaClienteRepository = sistemaClienteRepository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        try
        {
            var token = ExtractTokenFromHeader(context);

            var idSistemaCliente = _validadorTokenSistemaCliente.ValidarEPegarIdSistemaCliente(token);

            if (!await _sistemaClienteRepository.ExisteSistemaClienteAtivoComId(idSistemaCliente, cancellationToken))
                throw new UnauthorizedException(ResourceMessagesException.TOKEN_INVALIDO);
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = CreateUnauthorizedResult(ResourceMessagesException.TOKEN_EXPIRADO, true);
        }
        catch (ExceptionBase ex)
        {
            context.Result = CreateUnauthorizedResult(ex.Message);
        }
        catch
        {
            context.Result = CreateUnauthorizedResult(ResourceMessagesException.TOKEN_INVALIDO);
        }
    }

    private static string ExtractTokenFromHeader(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValues))
            throw new UnauthorizedException(ResourceMessagesException.SEM_TOKEN);

        var token = headerValues.ToString();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException(ResourceMessagesException.SEM_TOKEN);

        const string prefixoBearer = "Bearer ";
        if (!token.StartsWith(prefixoBearer, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedException(ResourceMessagesException.SEM_TOKEN);

        return token.AsSpan(prefixoBearer.Length).Trim().ToString();
    }

    private static UnauthorizedObjectResult CreateUnauthorizedResult(string message, bool tokenExpired = false)
    {
        return new UnauthorizedObjectResult(new ResponseErro(message) { TokenEstaExpirado = tokenExpired });
    }
}
