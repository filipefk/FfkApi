using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class PerfilAcessoRepository : IPerfilAcessoRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public PerfilAcessoRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public async Task<IList<PerfilAcesso>> PegarPorNomesAsync(IList<string> nomes, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.PerfisAcesso
                .Where(p => nomes.Contains(p.Nome))
                .ToListAsync(ct),
            cancellationToken
        );
    }
}