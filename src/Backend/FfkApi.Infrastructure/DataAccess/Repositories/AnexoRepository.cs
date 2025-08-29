using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class AnexoRepository : IAnexoRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AnexoRepository(
        FfkApiDbContext dbContext,
        AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Anexo> AsQueryable()
    {
        return _dbContext
            .Anexos
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Anexos.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Anexo anexo, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Anexos.AddAsync(anexo, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                var anexo = await _dbContext.Anexos.FindAsync([id, ct], ct);

                if (anexo is null)
                    return;

                _dbContext.Remove(anexo);
            },
            cancellationToken
        );
    }

    public async Task<Anexo?> PegarAnexoPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Anexos.FindAsync([id, ct], ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteAnexoComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Anexos.AnyAsync(anexo => anexo.Id.Equals(id), ct),
            cancellationToken
        );
    }
}