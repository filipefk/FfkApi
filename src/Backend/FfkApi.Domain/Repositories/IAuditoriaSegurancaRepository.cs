using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IAuditoriaSegurancaRepository
{
    Task Adicionar(AuditoriaSeguranca auditoriaSeguranca, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<AuditoriaSeguranca?> PegarAuditoriaSegurancaPorId(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteAuditoriaSegurancaComId(Guid idAuditoriaSeguranca, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    IQueryable<AuditoriaSeguranca> AsQueryable();
    Task<int> Limpar(uint dias, CancellationToken cancellationToken);
}
