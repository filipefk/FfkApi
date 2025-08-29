using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Credenciais;
using FfkApi.Domain.Security.Tokens;

namespace FfkApi.Infrastructure.Security.Tokens;

public class GeradorTokenAtivacao : IGeradorTokenAtivacao
{
    private readonly uint _tempoValidadeHoras;
    private readonly IGeradorToken _geradorToken;

    public GeradorTokenAtivacao(
        uint tempoValidadeHoras,
        IGeradorToken geradorToken)
    {
        _tempoValidadeHoras = tempoValidadeHoras;
        _geradorToken = geradorToken;
    }

    public uint TempoValidadeHoras => _tempoValidadeHoras;

    public string Gerar() => _geradorToken.GerarToken();

    public bool TokenValido(TokenAtivacao tokenAtivacao)
    {
        var validoAte = tokenAtivacao.BaseExpiracaoUtc.AddHours(_tempoValidadeHoras);
        return validoAte >= DateTime.UtcNow;
    }
}
