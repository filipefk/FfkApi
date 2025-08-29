using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class RefreshTokenRepositoryBuilder
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;

    public RefreshTokenRepositoryBuilder()
    {
        _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
    }

    public IRefreshTokenRepository Build()
    {
        return _refreshTokenRepository.Object;
    }

    public void SetupPegarRefreshTokenReturnsRefreshToken(string stringRefreshToken, RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _refreshTokenRepository.Setup(repository => repository.PegarRefreshToken(stringRefreshToken, cancellationToken)).ReturnsAsync(refreshToken);
    }
}