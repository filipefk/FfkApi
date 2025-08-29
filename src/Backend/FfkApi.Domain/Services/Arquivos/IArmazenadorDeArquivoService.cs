namespace FfkApi.Domain.Services.Arquivos;

public interface IArmazenadorDeArquivoService
{
    Task<string> SalvarAsync(Stream arquivoStream, string nomeArquivo, CancellationToken cancellationToken);
    Task<Stream?> ObterAsync(string nomeArquivoArmazenamento, CancellationToken cancellationToken);
    Task RemoverAsync(string nomeArquivoArmazenamento, CancellationToken cancellationToken);
    bool EstaDisponivel();
}
