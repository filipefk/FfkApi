using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IFeedRepository
{
    Task Adicionar(Feed feed, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<Feed?> PegarFeedPorId(Guid id, CancellationToken cancellationToken);
    Task<Feed?> PegarFeedPorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken);
    Task<bool> ExisteFeedComId(Guid idFeed, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken);
    IQueryable<Feed> AsQueryable();
    IQueryable<Feed> AsQueryable(Guid idOrganizacao);
}
