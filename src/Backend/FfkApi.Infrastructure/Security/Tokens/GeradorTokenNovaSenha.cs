using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Credenciais;
using FfkApi.Domain.Security.Tokens;

namespace FfkApi.Infrastructure.Security.Tokens;

public class GeradorTokenNovaSenha : IGeradorTokenNovaSenha
{
    private readonly uint _tempoValidadeHoras;
    private readonly IGeradorToken _geradorToken;

    public GeradorTokenNovaSenha(
        uint tempoValidadeHoras,
        IGeradorToken geradorToken)
    {
        _tempoValidadeHoras = tempoValidadeHoras;
        _geradorToken = geradorToken;
    }

    public uint TempoValidadeHoras => _tempoValidadeHoras;

    public string Gerar() => _geradorToken.GerarToken();

    public bool TokenValido(TokenNovaSenha tokenNovaSenha)
    {
        var validoAte = tokenNovaSenha.DataCriacaoUtc.AddHours(_tempoValidadeHoras);
        return validoAte >= DateTime.UtcNow;
    }
}
