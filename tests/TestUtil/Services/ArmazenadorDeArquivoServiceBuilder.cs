using FfkApi.Domain.Services.Arquivos;
using Moq;

namespace TestUtil.Services;

public class ArmazenadorDeArquivoServiceBuilder
{
    private readonly Mock<IArmazenadorDeArquivoService> _armazenadorDeArquivoService;

    public ArmazenadorDeArquivoServiceBuilder()
    {
        _armazenadorDeArquivoService = new Mock<IArmazenadorDeArquivoService>();
    }

    public IArmazenadorDeArquivoService Build()
    {
        return _armazenadorDeArquivoService.Object;
    }

    public void SetupSalvarAsyncReturnsNomeArquivoArmazenamento(string? nomeArquivo, CancellationToken cancellationToken)
    {
        nomeArquivo ??= "arquivo.txt";
        var nomeArquivoArmazenamento = $"{Guid.NewGuid()}{Path.GetExtension(nomeArquivo)}";
        _armazenadorDeArquivoService.Setup(service => service.SalvarAsync(It.IsAny<Stream>(), nomeArquivo, cancellationToken)).ReturnsAsync(nomeArquivoArmazenamento);
    }

    public void SetupObterAsyncReturnsStream(string nomeArquivoArmazenamento, CancellationToken cancellationToken)
    {
        _armazenadorDeArquivoService.Setup(service => service.ObterAsync(nomeArquivoArmazenamento, cancellationToken)).ReturnsAsync(new MemoryStream());
    }
}
