using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> PegarRefreshToken(string refreshToken, CancellationToken cancellationToken);

    Task SalvarNovoRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
}
