using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class IndisponibilidadeRepositoryBuilder
{
    private readonly Mock<IIndisponibilidadeRepository> _indisponibilidadeRepository;

    public IndisponibilidadeRepositoryBuilder()
    {
        _indisponibilidadeRepository = new Mock<IIndisponibilidadeRepository>();
    }

    public IIndisponibilidadeRepository Build()
    {
        return _indisponibilidadeRepository.Object;
    }

    public void SetupExisteIndisponibilidadeComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _indisponibilidadeRepository.Setup(repository => repository.ExisteIndisponibilidadeComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(Indisponibilidade indisponibilidade, CancellationToken cancellationToken)
    {
        _indisponibilidadeRepository.Setup(repository => repository.PegarIndisponibilidadePorId(indisponibilidade.Id, cancellationToken)).ReturnsAsync(indisponibilidade);
    }

    public void SetupPegarIndisponibilidadePorIdReturnsIndisponibilidade(Indisponibilidade indisponibilidade, Guid idOrganizacao, CancellationToken cancellationToken)
    {
        _indisponibilidadeRepository.Setup(repository => repository.PegarIndisponibilidadePorId(indisponibilidade.Id, idOrganizacao, cancellationToken)).ReturnsAsync(indisponibilidade);
    }

    public void SetupExisteIndisponibilidadeParaUsuarioNoPeriodoReturnsTrue(CancellationToken cancellationToken)
    {
        _indisponibilidadeRepository.Setup(repository => repository.ExisteIndisponibilidadeParaUsuarioNoPeriodo(
            It.IsAny<Guid>(),
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>(),
            It.IsAny<Guid?>(),
            cancellationToken))
        .ReturnsAsync(true);
    }
}
