using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class TokenNovaSenhaRepositoryBuilder
{
    private readonly Mock<ITokenNovaSenhaRepository> _tokenNovaSenhaRepository;

    public TokenNovaSenhaRepositoryBuilder()
    {
        _tokenNovaSenhaRepository = new Mock<ITokenNovaSenhaRepository>();
    }

    public ITokenNovaSenhaRepository Build()
    {
        return _tokenNovaSenhaRepository.Object;
    }

    public void SetupPegarTokenNovaSenhaPorTokenReturnsTokenNovaSenha(string stringTokenNovaSenha, TokenNovaSenha tokenNovaSenha, CancellationToken cancellationToken)
    {
        _tokenNovaSenhaRepository.Setup(repository => repository.PegarTokenNovaSenhaPorToken(stringTokenNovaSenha, cancellationToken)).ReturnsAsync(tokenNovaSenha);
    }

    public void SetupPegarTokenNovaSenhaPorUsuarioReturnsTokenNovaSenha(TokenNovaSenha tokenNovaSenha, CancellationToken cancellationToken)
    {
        _tokenNovaSenhaRepository.Setup(repository => repository.PegarTokenNovaSenhaPorUsuario(It.IsAny<Guid>(), cancellationToken)).ReturnsAsync(tokenNovaSenha);
    }

}
