using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Security.Tokens;

public interface IGeradorRefreshToken
{
    string Gerar();

    bool TokenValido(RefreshToken refreshToken);
}
