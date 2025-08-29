using FfkApi.Domain.Entities;
using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class SistemaClienteRepositoryBuilder
{
    private readonly Mock<ISistemaClienteRepository> _sistemaClienteRepository;

    public SistemaClienteRepositoryBuilder()
    {
        _sistemaClienteRepository = new Mock<ISistemaClienteRepository>();
    }

    public ISistemaClienteRepository Build()
    {
        return _sistemaClienteRepository.Object;
    }

    public void SetupExisteSistemaClienteComIdReturnsTrue(Guid id, CancellationToken cancellationToken)
    {
        _sistemaClienteRepository.Setup(repository => repository.ExisteSistemaClienteComId(id, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarSistemaClientePorIdReturnsSistemaCliente(SistemaCliente sistemaCliente, CancellationToken cancellationToken)
    {
        _sistemaClienteRepository.Setup(repository => repository.PegarSistemaClientePorId(sistemaCliente.Id, cancellationToken)).ReturnsAsync(sistemaCliente);
    }

    public void SetupExisteSistemaClienteComAppIdReturnsTrue(SistemaCliente sistemaCliente, CancellationToken cancellationToken)
    {
        _sistemaClienteRepository.Setup(repository => repository.ExisteSistemaClienteComAppId(sistemaCliente.AppId, cancellationToken)).ReturnsAsync(true);
    }

    public void SetupPegarSistemaClienteAtivoPorAppIdReturnsSistemaCliente(SistemaCliente sistemaCliente, CancellationToken cancellationToken)
    {
        _sistemaClienteRepository.Setup(repository => repository.PegarSistemaClienteAtivoPorAppId(sistemaCliente.AppId, cancellationToken)).ReturnsAsync(sistemaCliente);
    }

}
