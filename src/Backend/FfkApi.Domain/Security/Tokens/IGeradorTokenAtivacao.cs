using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Security.Tokens;

public interface IGeradorTokenAtivacao
{
    public uint TempoValidadeHoras { get; }
    string Gerar();
    bool TokenValido(TokenAtivacao tokenAtivacao);
}
