using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class EquipeRepositoryBuilder
{
    private readonly Mock<IEquipeRepository> _equipeRepository;

    public EquipeRepositoryBuilder()
    {
        _equipeRepository = new Mock<IEquipeRepository>();
    }

    public IEquipeRepository Build()
    {
        return _equipeRepository.Object;
    }

    public void SetupExisteEquipeComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _equipeRepository.Setup(repository => repository.ExisteEquipeComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarEquipePorIdReturnsEquipe(Equipe equipe, CancellationToken cancellationToken)
    {
        _equipeRepository.Setup(repository => repository.PegarEquipePorId(equipe.Id, cancellationToken)).ReturnsAsync(equipe);
    }

    public void SetupPegarEquipePorIdReturnsEquipe(Equipe equipe, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        _equipeRepository.Setup(repository => repository.PegarEquipePorId(equipe.Id, idOrganizacao, cancellationToken)).ReturnsAsync(equipe);
    }

    public void SetupPegarEquipePorNomeReturnsEquipe(Equipe equipe, CancellationToken cancellationToken)
    {
        _equipeRepository.Setup(repository => repository.PegarEquipePorNome(equipe.Nome, cancellationToken)).ReturnsAsync(equipe);
    }

    public void SetupExisteEquipeComNomeReturnsTrue(string nomeEquipe, string nomeOrganizacao, CancellationToken cancellationToken)
    {
        _equipeRepository.Setup(repository => repository.ExisteEquipeComNome(nomeEquipe, nomeOrganizacao, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarPorNomesNaOrganizacaoReturnsEquipes(
        List<Equipe> equipes,
        string nomeOrganizacao,
        CancellationToken cancellationToken)
    {
        var nomesEquipes = equipes.Select(equipe => equipe.Nome).ToList();
        _equipeRepository.Setup(repository => repository.PegarPorNomesNaOrganizacao(nomesEquipes, nomeOrganizacao, cancellationToken))
            .ReturnsAsync(equipes);
    }
}
