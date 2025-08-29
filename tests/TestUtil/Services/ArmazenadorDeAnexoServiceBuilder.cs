using FfkApi.Application.Services.Anexo;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Entities;
using Moq;

namespace TestUtil.Services;

public class ArmazenadorDeAnexoServiceBuilder
{
    private readonly Mock<IArmazenadorDeAnexoService> _armazenadorDeAnexoService;

    public ArmazenadorDeAnexoServiceBuilder()
    {
        _armazenadorDeAnexoService = new Mock<IArmazenadorDeAnexoService>();
    }

    public IArmazenadorDeAnexoService Build()
    {
        return _armazenadorDeAnexoService.Object;
    }

    public void SetupSalvarAsyncReturnsAnexo(Anexo anexo, CancellationToken cancellationToken)
    {
        _armazenadorDeAnexoService.Setup(service => service.SalvarAsync(It.IsAny<RequestCadastrarAnexo>(), cancellationToken)).ReturnsAsync(anexo);
    }
}
