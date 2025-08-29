using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class TokenAtivacaoRepository : ITokenAtivacaoRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public TokenAtivacaoRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public void ApagarTokensDoUsuario(Guid idUsuario)
    {
        var tokens = _dbContext.TokensAtivacao.Where(token => token.IdUsuario.Equals(idUsuario));
        _dbContext.TokensAtivacao.RemoveRange(tokens);
    }

    public async Task<TokenAtivacao?> PegarTokenAtivacaoPorToken(string tokenAtivacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .TokensAtivacao
                .AsNoTracking()
                .Include(token => token.Usuario)
                .FirstOrDefaultAsync(token => token.Valor.Equals(tokenAtivacao), ct),
            cancellationToken
        );
    }

    public async Task<TokenAtivacao?> PegarTokenAtivacaoPorUsuario(Guid idUsuario, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .TokensAtivacao
                .Include(token => token.Usuario)
                .FirstOrDefaultAsync(token => token.Usuario.Id.Equals(idUsuario), ct),
            cancellationToken
        );
    }

    public async Task SalvarNovoTokenAtivacao(TokenAtivacao tokenAtivacao, CancellationToken cancellationToken)
    {
        ApagarTokensDoUsuario(tokenAtivacao.IdUsuario);
        await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.TokensAtivacao.AddAsync(tokenAtivacao, ct),
            cancellationToken
        );
    }

    public async Task RedefinirDataExpiracaoEMarcarParaEnviarNovoEmail(Guid idTokenAtivacao, CancellationToken cancellationToken)
    {
        var token = await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.TokensAtivacao.FindAsync([idTokenAtivacao, ct], ct),
            cancellationToken
        );
        token!.BaseExpiracaoUtc = DateTime.UtcNow;
        token.EmailEnviado = false;
        token.ErroEnvioEmail = null;
    }
}