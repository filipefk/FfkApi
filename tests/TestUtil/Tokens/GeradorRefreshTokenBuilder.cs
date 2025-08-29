using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Credenciais;
using FfkApi.Infrastructure.Security.Tokens;
using Moq;

namespace TestUtil.Tokens;

public class GeradorRefreshTokenBuilder
{
    private readonly Mock<IGeradorRefreshToken> _geradorRefreshToken;
    private readonly GeradorRefreshToken _geradorRefreshTokenReal;

    public GeradorRefreshTokenBuilder()
    {
        _geradorRefreshToken = new Mock<IGeradorRefreshToken>();
        _geradorRefreshTokenReal = new GeradorRefreshToken(7, new GeradorToken());
        _geradorRefreshToken.Setup(g => g.Gerar()).Returns(_geradorRefreshTokenReal.Gerar());
    }

    public IGeradorRefreshToken Build()
    {
        return _geradorRefreshToken.Object;
    }

    public void SetupTokenValidoReturnsTrue(RefreshToken refreshToken)
    {
        _geradorRefreshToken.Setup(geradorRefreshToken => geradorRefreshToken.TokenValido(refreshToken)).Returns(true);
    }
}
