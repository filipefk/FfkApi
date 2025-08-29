using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class OrganizacaoRepository : IOrganizacaoRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public OrganizacaoRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Organizacao> AsQueryable()
    {
        return _dbContext
            .Organizacoes
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Organizacao organizacao, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.AddAsync(organizacao, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        var organizacao = await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.FindAsync([id, ct], ct),
            cancellationToken
        );

        if (organizacao is null)
            return;

        _dbContext.Remove(organizacao);
    }

    public async Task<Organizacao?> PegarOrganizacaoPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.FindAsync([id, ct], ct),
            cancellationToken
        );
    }

    public async Task<Organizacao?> PegarOrganizacaoPorNome(string nome, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext
                .Organizacoes
                .FirstOrDefaultAsync(organizacao => organizacao.Nome.Equals(nome), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteOrganizacaoComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.AnyAsync(organizacao => organizacao.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteOrganizacaoComNome(string nome, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            async ct => await _dbContext.Organizacoes.AnyAsync(organizacao => organizacao.Nome.Equals(nome), ct),
            cancellationToken
        );
    }
}