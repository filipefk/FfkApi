using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class TokenNovaSenhaRepository : ITokenNovaSenhaRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TokenNovaSenhaRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public void ApagarTokensDoUsuario(Guid idUsuario)
    {
        var tokens = _dbContext.TokensNovaSenha.Where(token => token.IdUsuario.Equals(idUsuario));
        _dbContext.TokensNovaSenha.RemoveRange(tokens);
    }

    public async Task<TokenNovaSenha?> PegarTokenNovaSenhaPorToken(string tokenNovaSenha, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .TokensNovaSenha
                .AsNoTracking()
                .Include(token => token.Usuario)
                .FirstOrDefaultAsync(token => token.Valor.Equals(tokenNovaSenha), ct),
            cancellationToken);
    }

    public async Task<TokenNovaSenha?> PegarTokenNovaSenhaPorUsuario(Guid idUsuario, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .TokensNovaSenha
                .Include(token => token.Usuario)
                .FirstOrDefaultAsync(token => token.IdUsuario.Equals(idUsuario), ct),
            cancellationToken);
    }

    public async Task SalvarNovoTokenNovaSenha(TokenNovaSenha tokenNovaSenha, CancellationToken cancellationToken)
    {
        ApagarTokensDoUsuario(tokenNovaSenha.IdUsuario);
        await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.TokensNovaSenha.AddAsync(tokenNovaSenha, ct),
            cancellationToken);
    }
}