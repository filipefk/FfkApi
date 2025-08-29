using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class SistemaClienteRepository : ISistemaClienteRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public SistemaClienteRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<SistemaCliente> AsQueryable()
    {
        return _dbContext
            .SistemasCliente
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(SistemaCliente sistemaCliente, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
        {
            await _dbContext.SistemasCliente.AddAsync(sistemaCliente, ct);
        }, cancellationToken);
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        var sistemaCliente = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.FindAsync([id, ct], ct),
            cancellationToken
        );

        if (sistemaCliente is null)
            return;

        _dbContext.Remove(sistemaCliente);
    }

    public async Task<SistemaCliente?> PegarSistemaClientePorId(Guid idSistemaCliente, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.FindAsync([idSistemaCliente, ct], ct),
            cancellationToken
        );
    }

    public async Task<SistemaCliente?> PegarSistemaClienteAtivoPorAppId(Guid appId, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.FirstOrDefaultAsync(sistemaCliente =>
                sistemaCliente.Status == Domain.Enums.StatusSistemaCliente.Ativo &&
                sistemaCliente.AppId.Equals(appId)
            , ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteSistemaClienteComId(Guid idSistemaCliente, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.AnyAsync(sistemaCliente => sistemaCliente.Id.Equals(idSistemaCliente), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteSistemaClienteAtivoComId(Guid idSistemaCliente, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.AnyAsync(sistemaCliente =>
                sistemaCliente.Status == Domain.Enums.StatusSistemaCliente.Ativo &&
                sistemaCliente.Id.Equals(idSistemaCliente)
            , ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteSistemaClienteComAppId(Guid appId, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.SistemasCliente.AnyAsync(sistemaCliente => sistemaCliente.AppId.Equals(appId), ct),
            cancellationToken
        );
    }
}