using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Security.Tokens;

public interface IGeradorTokenNovaSenha
{
    public uint TempoValidadeHoras { get; }
    string Gerar();
    bool TokenValido(TokenNovaSenha tokenAtivacao);
}
