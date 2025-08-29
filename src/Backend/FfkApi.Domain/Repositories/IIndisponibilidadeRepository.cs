using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IIndisponibilidadeRepository
{
    Task Adicionar(Indisponibilidade indisponibilidade, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<Indisponibilidade?> PegarIndisponibilidadePorId(Guid id, CancellationToken cancellationToken);
    Task<Indisponibilidade?> PegarIndisponibilidadePorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken);
    Task<bool> ExisteIndisponibilidadeComId(Guid idIndisponibilidade, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken);
    IQueryable<Indisponibilidade> AsQueryable();
    Task<bool> ExisteIndisponibilidadeParaUsuarioNoPeriodo(Guid idUsuario, DateOnly dataInicial, DateOnly dataFinal, Guid? ignorarEsta, CancellationToken cancellationToken);
}
