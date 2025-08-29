using FfkApi.Domain.Repositories;
using Polly.Retry;

namespace FfkApi.Infrastructure.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FfkApiDbContext _dbContext;
    private readonly AsyncRetryPolicy _retryPolicy;

    public UnitOfWork(FfkApiDbContext dbContext, AsyncRetryPolicy retryPolicy)
    {
        _dbContext = dbContext;
        _retryPolicy = retryPolicy;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        });
    }
}