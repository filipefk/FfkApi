using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class PerfilAcessoRepositoryBuilder
{
    private readonly Mock<IPerfilAcessoRepository> _perfilAcessoRepository;

    public PerfilAcessoRepositoryBuilder()
    {
        _perfilAcessoRepository = new Mock<IPerfilAcessoRepository>();
    }

    public IPerfilAcessoRepository Build()
    {
        return _perfilAcessoRepository.Object;
    }

    public void SetupPegarPorNomesAsyncReturnsPerfis(IList<string> nomes, IList<PerfilAcesso> perfis, CancellationToken cancellationToken)
    {
        _perfilAcessoRepository.Setup(repository => repository.PegarPorNomesAsync(nomes, cancellationToken)).ReturnsAsync(perfis);
    }
}
