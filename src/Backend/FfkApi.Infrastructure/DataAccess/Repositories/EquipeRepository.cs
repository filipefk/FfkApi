using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class EquipeRepository : IEquipeRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public EquipeRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Equipe> AsQueryable()
    {
        return _dbContext
            .Equipes
            .Include(equipe => equipe.Membros).ThenInclude(membro => membro.Usuario)
            .Include(equipe => equipe.Organizacao)
            .AsQueryable();
    }

    public IQueryable<Equipe> AsQueryable(Guid idOrganizacao)
    {
        return _dbContext
            .Equipes
            .Include(equipe => equipe.Membros).ThenInclude(membro => membro.Usuario)
            .Include(equipe => equipe.Organizacao)
            .Where(equipe => equipe.Organizacao.Id == idOrganizacao)
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes
                .Where(equipe => equipe.Organizacao.Id == idOrganizacao)
                .LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Equipe equipe, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes.AddAsync(equipe, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
        {
            var equipe = await _dbContext.Equipes.FindAsync([id, ct], ct);
            if (equipe is null)
                return;
            _dbContext.Remove(equipe);
        }, cancellationToken);
    }

    public async Task<Equipe?> PegarEquipePorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .Equipes
                .Include(equipe => equipe.Membros).ThenInclude(membro => membro.Usuario)
                .Include(equipe => equipe.Organizacao)
                .FirstOrDefaultAsync(equipe => equipe.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<Equipe?> PegarEquipePorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .Equipes
                .Include(equipe => equipe.Membros).ThenInclude(membro => membro.Usuario)
                .Include(equipe => equipe.Organizacao)
                .FirstOrDefaultAsync(equipe => equipe.Id.Equals(id) && equipe.Organizacao.Id.Equals(idOrganizacao), ct),
            cancellationToken
        );
    }

    public async Task<Equipe?> PegarEquipePorNome(string nome, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .Equipes
                .FirstOrDefaultAsync(equipe => equipe.Nome.Equals(nome), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteEquipeComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes.AnyAsync(equipe => equipe.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteEquipeComNome(string nome, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes.AnyAsync(equipe => equipe.Nome.Equals(nome), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteEquipeComNome(string nomeEquipe, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes.AnyAsync(equipe =>
                equipe.Nome.Equals(nomeEquipe) &&
                equipe.Organizacao.Nome.Equals(nomeOrganizacao), ct),
            cancellationToken
        );
    }

    public async Task<IList<Equipe>> PegarPorNomesNaOrganizacao(IList<string> nomes, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Equipes
                .Where(e => nomes.Contains(e.Nome) && e.Organizacao.Nome == nomeOrganizacao)
                .ToListAsync(ct),
            cancellationToken
        );
    }

    public async Task AdicionarMembro(MembroEquipe membro, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.MembrosEquipe.AddAsync(membro, ct),
            cancellationToken
        );
    }
}