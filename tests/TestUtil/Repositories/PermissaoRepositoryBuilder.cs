using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class PermissaoRepositoryBuilder
{
    private readonly Mock<IPermissaoRepository> _permissaoRepository;

    public PermissaoRepositoryBuilder()
    {
        _permissaoRepository = new Mock<IPermissaoRepository>();
    }

    public IPermissaoRepository Build()
    {
        return _permissaoRepository.Object;
    }

    public void SetupPegarPorNomesAsyncReturnsPermissoes(IList<string> nomes, IList<Permissao> permissoes, CancellationToken cancellationToken)
    {
        _permissaoRepository.Setup(repository => repository.PegarPorNomesAsync(nomes, cancellationToken)).ReturnsAsync(permissoes);
    }
}
