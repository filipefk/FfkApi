using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IAnexoRepository
{
    Task Adicionar(Anexo anexo, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<Anexo?> PegarAnexoPorId(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteAnexoComId(Guid idAnexo, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    IQueryable<Anexo> AsQueryable();
}
