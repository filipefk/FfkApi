using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class FeedRepository : IFeedRepository
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public FeedRepository(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public IQueryable<Feed> AsQueryable()
    {
        return _dbContext
            .Feeds
            .Include(feed => feed.Anexos)
            .Include(feed => feed.VisibilidadeUsuarios)
            .Include(feed => feed.VisibilidadeEquipes)
            .Include(feed => feed.Organizacao)
            .AsQueryable();
    }

    public IQueryable<Feed> AsQueryable(Guid idOrganizacao)
    {
        return _dbContext
            .Feeds
            .Include(feed => feed.Anexos)
            .Include(feed => feed.VisibilidadeUsuarios)
            .Include(feed => feed.VisibilidadeEquipes)
            .Include(feed => feed.Organizacao)
            .Where(feed => feed.Organizacao.Id == idOrganizacao)
            .AsQueryable();
    }

    public async Task<long> QuantidadeTotal(CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Feeds.LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Feeds
                .Where(feed => feed.Organizacao.Id == idOrganizacao)
                .LongCountAsync(ct),
            cancellationToken
        );
    }

    public async Task Adicionar(Feed feed, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Feeds.AddAsync(feed, ct),
            cancellationToken
        );
    }

    public async Task Excluir(Guid id, CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async ct =>
        {
            var feed = await _dbContext.Feeds.FindAsync([id, ct], ct);

            if (feed is null)
                return;

            _dbContext.Remove(feed);
        }, cancellationToken);
    }

    public async Task<Feed?> PegarFeedPorId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .Feeds
                .Include(feed => feed.Anexos)
                .Include(feed => feed.VisibilidadeUsuarios)
                .Include(feed => feed.VisibilidadeEquipes)
                .Include(feed => feed.Organizacao)
                .FirstOrDefaultAsync(feed => feed.Id.Equals(id), ct),
            cancellationToken
        );
    }

    public async Task<Feed?> PegarFeedPorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext
                .Feeds
                .Include(feed => feed.Anexos)
                .Include(feed => feed.VisibilidadeUsuarios)
                .Include(feed => feed.VisibilidadeEquipes)
                .Include(feed => feed.Organizacao)
                .FirstOrDefaultAsync(feed => feed.Id.Equals(id) && feed.Organizacao.Id.Equals(idOrganizacao), ct),
            cancellationToken
        );
    }

    public async Task<bool> ExisteFeedComId(Guid id, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async ct =>
            await _dbContext.Feeds.AnyAsync(feed => feed.Id.Equals(id), ct),
            cancellationToken
        );
    }
}