using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Credenciais;
using FfkApi.Infrastructure.Security.Tokens;
using Moq;

namespace TestUtil.Tokens;

public class GeradorTokenNovaSenhaBuilder
{
    private readonly Mock<IGeradorTokenNovaSenha> _geradorTokenNovaSenha;
    private readonly GeradorTokenNovaSenha _geradorTokenNovaSenhaReal;

    public GeradorTokenNovaSenhaBuilder()
    {
        _geradorTokenNovaSenha = new Mock<IGeradorTokenNovaSenha>();
        _geradorTokenNovaSenhaReal = new GeradorTokenNovaSenha(1, new GeradorToken());
        _geradorTokenNovaSenha.Setup(g => g.Gerar()).Returns(_geradorTokenNovaSenhaReal.Gerar());
    }

    public IGeradorTokenNovaSenha Build()
    {
        return _geradorTokenNovaSenha.Object;
    }

    public void SetupTokenValidoReturnsTrue(TokenNovaSenha tokenNovaSenha)
    {
        _geradorTokenNovaSenha.Setup(geradorTokenNovaSenha => geradorTokenNovaSenha.TokenValido(tokenNovaSenha)).Returns(true);
    }
}
