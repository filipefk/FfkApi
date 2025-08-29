using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class OrganizacaoRepositoryBuilder
{
    private readonly Mock<IOrganizacaoRepository> _organizacaoRepository;

    public OrganizacaoRepositoryBuilder()
    {
        _organizacaoRepository = new Mock<IOrganizacaoRepository>();
    }

    public IOrganizacaoRepository Build()
    {
        return _organizacaoRepository.Object;
    }

    public void SetupExisteOrganizacaoComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _organizacaoRepository.Setup(repository => repository.ExisteOrganizacaoComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarOrganizacaoPorIdReturnsOrganizacao(Organizacao organizacao, CancellationToken cancellationToken)
    {
        _organizacaoRepository.Setup(repository => repository.PegarOrganizacaoPorId(organizacao.Id, cancellationToken)).ReturnsAsync(organizacao);
    }

    public void SetupPegarOrganizacaoPorNomeReturnsOrganizacao(Organizacao organizacao, CancellationToken cancellationToken)
    {
        _organizacaoRepository.Setup(repository => repository.PegarOrganizacaoPorNome(organizacao.Nome, cancellationToken)).ReturnsAsync(organizacao);
    }

    public void SetupExisteOrganizacaoComNomeReturnsTrue(string nomeOrganizacao, CancellationToken cancellationToken)
    {
        _organizacaoRepository.Setup(repository => repository.ExisteOrganizacaoComNome(nomeOrganizacao, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupExcluirThrowsInvalidOperationException(Guid id, CancellationToken cancellationToken)
    {
        _organizacaoRepository.Setup(repository => repository.Excluir(id, cancellationToken)).ThrowsAsync(new InvalidOperationException());
    }
}
