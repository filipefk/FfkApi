using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class AnexoRepositoryBuilder
{
    private readonly Mock<IAnexoRepository> _anexoRepository;

    public AnexoRepositoryBuilder()
    {
        _anexoRepository = new Mock<IAnexoRepository>();
    }

    public IAnexoRepository Build()
    {
        return _anexoRepository.Object;
    }

    public void SetupExisteAnexoComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _anexoRepository.Setup(repository => repository.ExisteAnexoComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarAnexoPorIdReturnsAnexo(Anexo anexo, CancellationToken cancellationToken)
    {
        _anexoRepository.Setup(repository => repository.PegarAnexoPorId(anexo.Id, cancellationToken)).ReturnsAsync(anexo);
    }
}
