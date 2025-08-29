using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class TokenAtivacaoRepositoryBuilder
{
    private readonly Mock<ITokenAtivacaoRepository> _tokenAtivacaoRepository;

    public TokenAtivacaoRepositoryBuilder()
    {
        _tokenAtivacaoRepository = new Mock<ITokenAtivacaoRepository>();
    }

    public ITokenAtivacaoRepository Build()
    {
        return _tokenAtivacaoRepository.Object;
    }

    public void SetupPegarTokenAtivacaoPorTokenReturnsTokenAtivacao(string stringTokenAtivacao, TokenAtivacao tokenAtivacao, CancellationToken cancellationToken)
    {
        _tokenAtivacaoRepository.Setup(repository => repository.PegarTokenAtivacaoPorToken(stringTokenAtivacao, cancellationToken)).ReturnsAsync(tokenAtivacao);
    }

    public void SetupPegarTokenAtivacaoPorUsuarioReturnsTokenAtivacao(Guid idUsuario, TokenAtivacao tokenAtivacao, CancellationToken cancellationToken)
    {
        _tokenAtivacaoRepository.Setup(repository => repository.PegarTokenAtivacaoPorUsuario(idUsuario, cancellationToken)).ReturnsAsync(tokenAtivacao);
    }

    public void SetupPegarTokenAtivacaoPorUsuarioReturnsTokenAtivacao(TokenAtivacao tokenAtivacao, CancellationToken cancellationToken)
    {
        _tokenAtivacaoRepository.Setup(repository => repository.PegarTokenAtivacaoPorUsuario(It.IsAny<Guid>(), cancellationToken)).ReturnsAsync(tokenAtivacao);
    }
}
