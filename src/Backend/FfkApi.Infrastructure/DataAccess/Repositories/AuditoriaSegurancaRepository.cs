using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class AuditoriaSegurancaRepository : IAuditoriaSegurancaRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AuditoriaSegurancaRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<AuditoriaSeguranca> AsQueryable()
    {
        return _dbContext
            .AuditoriasSeguranca
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.AuditoriasSeguranca.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(AuditoriaSeguranca auditoriaSeguranca, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.AuditoriasSeguranca.AddAsync(auditoriaSeguranca, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                var auditoriaSeguranca = await _dbContext.AuditoriasSeguranca.FindAsync([id, ct], ct);

                if (auditoriaSeguranca is null)
                    return;

                _dbContext.Remove(auditoriaSeguranca);
            },
            cancellationToken
        );
    }

    public async Task<AuditoriaSeguranca?> PegarAuditoriaSegurancaPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.AuditoriasSeguranca.FindAsync([id, ct], ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteAuditoriaSegurancaComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.AuditoriasSeguranca.AnyAsync(auditoriaSeguranca => auditoriaSeguranca.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<int> Limpar(uint dias, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                var dataLimite = DateTime.UtcNow.AddDays(-dias);

                var registrosParaExcluir = await _dbContext.AuditoriasSeguranca
                    .Where(a => a.DataCriacaoUtc <= dataLimite)
                    .ToListAsync(ct);

                var quant = registrosParaExcluir.Count;

                if (quant > 0)
                {
                    _dbContext.AuditoriasSeguranca.RemoveRange(registrosParaExcluir);
                }

                return quant;
            },
            cancellationToken
        );
    }
}