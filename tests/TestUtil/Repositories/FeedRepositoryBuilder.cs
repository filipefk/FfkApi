using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class FeedRepositoryBuilder
{
    private readonly Mock<IFeedRepository> _feedRepository;

    public FeedRepositoryBuilder()
    {
        _feedRepository = new Mock<IFeedRepository>();
    }

    public IFeedRepository Build()
    {
        return _feedRepository.Object;
    }

    public void SetupExisteFeedComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _feedRepository.Setup(repository => repository.ExisteFeedComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarFeedPorIdReturnsFeed(Feed feed, CancellationToken cancellationToken)
    {
        _feedRepository.Setup(repository => repository.PegarFeedPorId(feed.Id, cancellationToken)).ReturnsAsync(feed);
    }

    public void SetupPegarFeedPorIdReturnsFeed(Feed feed, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        _feedRepository.Setup(repository => repository.PegarFeedPorId(feed.Id, idOrganizacao, cancellationToken)).ReturnsAsync(feed);
    }
}
