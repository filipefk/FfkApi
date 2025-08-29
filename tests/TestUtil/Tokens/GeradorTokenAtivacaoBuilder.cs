using FfkApi.Domain.Entities;
using FfkApi.Domain.Security.Tokens;
using FfkApi.Infrastructure.Security.Credenciais;
using FfkApi.Infrastructure.Security.Tokens;
using Moq;

namespace TestUtil.Tokens;

public class GeradorTokenAtivacaoBuilder
{
    private readonly Mock<IGeradorTokenAtivacao> _geradorTokenAtivacao;
    private readonly GeradorTokenAtivacao _geradorTokenAtivacaoReal;

    public GeradorTokenAtivacaoBuilder()
    {
        _geradorTokenAtivacao = new Mock<IGeradorTokenAtivacao>();
        _geradorTokenAtivacaoReal = new GeradorTokenAtivacao(12, new GeradorToken());
        _geradorTokenAtivacao.Setup(g => g.Gerar()).Returns(_geradorTokenAtivacaoReal.Gerar());
    }

    public IGeradorTokenAtivacao Build()
    {
        return _geradorTokenAtivacao.Object;
    }

    public void SetupTokenValidoReturnsTrue(TokenAtivacao tokenAtivacao)
    {
        _geradorTokenAtivacao.Setup(geradorTokenAtivacao => geradorTokenAtivacao.TokenValido(tokenAtivacao)).Returns(true);
    }
}
