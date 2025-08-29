using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Credenciais;
using FfkApi.Domain.Security.Tokens;

namespace FfkApi.Infrastructure.Security.Tokens;

public class GeradorRefreshToken : IGeradorRefreshToken
{
    private readonly uint _tempoValidadeDias;
    private readonly IGeradorToken _geradorToken;

    public GeradorRefreshToken(
        uint tempoValidadeDias,
        IGeradorToken geradorToken)
    {
        _tempoValidadeDias = tempoValidadeDias;
        _geradorToken = geradorToken;
    }

    public string Gerar() => _geradorToken.GerarToken();

    public bool TokenValido(RefreshToken refreshToken)
    {
        var validoAteDia = refreshToken.DataCriacaoUtc.AddDays(_tempoValidadeDias);
        return validoAteDia >= DateTime.UtcNow;
    }

}
