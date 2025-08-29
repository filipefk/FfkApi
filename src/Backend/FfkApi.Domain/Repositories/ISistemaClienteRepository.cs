using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface ISistemaClienteRepository
{
    Task Adicionar(SistemaCliente sistemaCliente, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<SistemaCliente?> PegarSistemaClientePorId(Guid id, CancellationToken cancellationToken);
    Task<SistemaCliente?> PegarSistemaClienteAtivoPorAppId(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteSistemaClienteComId(Guid idSistemaCliente, CancellationToken cancellationToken);
    Task<bool> ExisteSistemaClienteAtivoComId(Guid idSistemaCliente, CancellationToken cancellationToken);
    Task<bool> ExisteSistemaClienteComAppId(Guid appId, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    IQueryable<SistemaCliente> AsQueryable();
}
