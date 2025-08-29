using FfkApi.Domain.Entities;

namespace FfkApi.Domain.Repositories;

public interface IEquipeRepository
{
    Task Adicionar(Equipe equipe, CancellationToken cancellationToken);
    Task Excluir(Guid id, CancellationToken cancellationToken);
    Task<Equipe?> PegarEquipePorId(Guid id, CancellationToken cancellationToken);
    Task<Equipe?> PegarEquipePorId(Guid id, Guid idOrganizacao, CancellationToken cancellationToken);
    Task<Equipe?> PegarEquipePorNome(string nome, CancellationToken cancellationToken);
    Task<bool> ExisteEquipeComId(Guid idEquipe, CancellationToken cancellationToken);
    Task<bool> ExisteEquipeComNome(string nome, CancellationToken cancellationToken);
    Task<bool> ExisteEquipeComNome(string nome, string nomeOrganizacao, CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(CancellationToken cancellationToken);
    Task<long> QuantidadeTotal(Guid idOrganizacao, CancellationToken cancellationToken);
    IQueryable<Equipe> AsQueryable();
    IQueryable<Equipe> AsQueryable(Guid idOrganizacao);
    Task AdicionarMembro(MembroEquipe membro, CancellationToken cancellationToken);
    Task<IList<Equipe>> PegarPorNomesNaOrganizacao(IList<string> nomes, string nomeOrganizacao, CancellationToken cancellationToken);
}
