using FfkApi.Domain.Repositories;
using Moq;

namespace TestUtil.Repositories;

public class AuditoriaSegurancaRepositoryBuilder
{
    private readonly Mock<IAuditoriaSegurancaRepository> _auditoriaSegurancaRepository;

    public AuditoriaSegurancaRepositoryBuilder()
    {
        _auditoriaSegurancaRepository = new Mock<IAuditoriaSegurancaRepository>();
    }

    public IAuditoriaSegurancaRepository Build()
    {
        return _auditoriaSegurancaRepository.Object;
    }

    public void SetupLimparReturnsQuant(uint dias, int quant, CancellationToken cancellationToken)
    {
        _auditoriaSegurancaRepository.Setup(repository => repository.Limpar(dias, cancellationToken)).ReturnsAsync(quant);
    }
}
