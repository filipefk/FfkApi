using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IOrganizacaoRepository
{
    Task Adicionar(Organizacao organizacao, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<Organizacao?> PegarOrganizacaoPorId(Guid id, CancellationToken cancellationToken);
    Task<Organizacao?> PegarOrganizacaoPorNome(string nome, CancellationToken cancellationToken);
    Task<bool> ExisteOrganizacaoComId(Guid idOrganizacao, CancellationToken cancellationToken);
    Task<bool> ExisteOrganizacaoComNome(string nome, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    IQueryable<Organizacao> AsQueryable();
}
