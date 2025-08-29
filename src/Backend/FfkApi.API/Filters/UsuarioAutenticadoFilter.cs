using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Domain.Services.Acesso;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace FfkApi.API.Filters;

public class UsuarioAutenticadoFilter : IAsyncAuthorizationFilter
{
    private readonly IValidadorTokenUsuario _validadorTokenUsuario;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAcessoService _acessoService;
    private readonly string? _permissao;

    public UsuarioAutenticadoFilter(
        IValidadorTokenUsuario validadorTokenUsuario,
        IUsuarioRepository usuarioRepository,
        IAcessoService acessoService,
        string? permissao = null)
    {
        _validadorTokenUsuario = validadorTokenUsuario;
        _usuarioRepository = usuarioRepository;
        _acessoService = acessoService;
        _permissao = permissao;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        try
        {
            var token = ExtractTokenFromHeader(context);

            var idUsuario = _validadorTokenUsuario.ValidarEPegarIdUsuario(token);

            if (!await _usuarioRepository.ExisteUsuarioAptoComId(idUsuario, cancellationToken))
                throw new UnauthorizedException(ResourceMessagesException.TOKEN_INVALIDO);

            if (!string.IsNullOrWhiteSpace(_permissao) &&
                !await _acessoService.UsuarioTemPermissaoAsync(idUsuario, _permissao))
            {
                context.Result = CreateForbiddenResult(_permissao);
                return;
            }
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

    private static ObjectResult CreateForbiddenResult(string permissao)
    {
        return new ObjectResult(new ResponseErro(ResourceMessagesException.SEM_PERMISSAO.Replace("{permissao}", permissao)))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    private static UnauthorizedObjectResult CreateUnauthorizedResult(string message, bool tokenExpired = false)
    {
        return new UnauthorizedObjectResult(new ResponseErro(message) { TokenEstaExpirado = tokenExpired });
    }
}
