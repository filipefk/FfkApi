using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RefreshTokenRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public async Task<RefreshToken?> PegarRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .RefreshTokens
                .AsNoTracking()
                .Include(token => token.Usuario)
                .FirstOrDefaultAsync(token => token.Valor.Equals(refreshToken)
                    && (token.Usuario.Status == Domain.Enums.StatusUsuario.Ativo
                        || token.Usuario.Status == Domain.Enums.StatusUsuario.Ausente)
                    , ct),
            cancellationToken);
    }

    public async Task SalvarNovoRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
        {
            var tokens = _dbContext.RefreshTokens.Where(token => token.IdUsuario.Equals(refreshToken.IdUsuario));
            _dbContext.RefreshTokens.RemoveRange(tokens);
            await _dbContext.RefreshTokens.AddAsync(refreshToken, ct);
        }, cancellationToken);
    }
}