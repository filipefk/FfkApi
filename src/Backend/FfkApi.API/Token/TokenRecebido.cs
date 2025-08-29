using FfkApi.Domain.Security.Tokens;

namespace FfkApi.API.Token;

public class TokenRecebido : ITokenRecebido
{
    private readonly IHttpContextAccessor _contextAccessor;

    public TokenRecebido(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string Token()
    {
        var authorization = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

        var retorno = string.IsNullOrWhiteSpace(authorization) || authorization.Length < "Bearer".Length ? string.Empty : authorization["Bearer".Length..].Trim();

        return retorno;
    }
}