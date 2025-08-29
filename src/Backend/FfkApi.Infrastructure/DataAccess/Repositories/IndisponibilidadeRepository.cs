using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class IndisponibilidadeRepository : IIndisponibilidadeRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public IndisponibilidadeRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Indisponibilidade> AsQueryable()
    {
        return _dbContext
            .Indisponibilidades
            .Include(i => i.Usuario).ThenInclude(u => u.Organizacao)
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Indisponibilidades.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Indisponibilidades
                .Where(i => i.Usuario.Organizacao.Id == idOrganizacao)
                .LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Indisponibilidade indisponibilidade, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Indisponibilidades.AddAsync(indisponibilidade, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct =>
            {
                var indisponibilidade = await _dbContext.Indisponibilidades.FindAsync([id, ct], ct);

                if (indisponibilidade is null)
                    return;

                _dbContext.Remove(indisponibilidade);
            },
            cancellationToken
        );
    }

    public async Task<Indisponibilidade?> PegarIndisponibilidadePorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Indisponibilidades
                .Include(i => i.Usuario).ThenInclude(u => u.Organizacao)
                .FirstOrDefaultAsync(i => i.Id.Equals(id), cancellationToken: ct),
            cancellationToken
        );
    }

    public async Task<Indisponibilidade?> PegarIndisponibilidadePorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Indisponibilidades
                .Include(i => i.Usuario).ThenInclude(u => u.Organizacao)
                .FirstOrDefaultAsync(i => i.Id.Equals(id) && i.Usuario.Organizacao.Id.Equals(idOrganizacao), cancellationToken: ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteIndisponibilidadeComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Indisponibilidades.AnyAsync(indisponibilidade => indisponibilidade.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteIndisponibilidadeParaUsuarioNoPeriodo(Guid idUsuario, DateOnly dataInicial, DateOnly dataFinal, Guid? ignorarEsta, CancellationToken cancellationToken)
    {
        return await _dbContext.Indisponibilidades
            .AnyAsync(i => i.IdUsuario == idUsuario &&
                (ignorarEsta == null || i.Id != ignorarEsta) &&
                i.DataInicial <= dataFinal && dataInicial <= i.DataFinal,
                cancellationToken);
    }
}
