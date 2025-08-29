using FfkApi.Domain.Services.Arquivos;
using System.Collections.Concurrent;

namespace TestUtil.Services;

public class ArmazenadorDeArquivoEmMemoriaService : IArmazenadorDeArquivoService
{
    private readonly ConcurrentDictionary<string, byte[]> _storage = new();

    public async Task<string> SalvarAsync(Stream arquivoStream, string nomeArquivo, CancellationToken cancellationToken)
    {
        var nomeArquivoArmazenamento = $"{Guid.NewGuid()}{Path.GetExtension(nomeArquivo)}";
        using var memoryStream = new MemoryStream();
        await arquivoStream.CopyToAsync(memoryStream, cancellationToken);
        _storage[nomeArquivoArmazenamento] = memoryStream.ToArray();
        return nomeArquivoArmazenamento;
    }

    public Task<Stream?> ObterAsync(string nomeArquivoArmazenamento, CancellationToken _)
    {
        if (_storage.TryGetValue(nomeArquivoArmazenamento, out var data))
        {
            var stream = new MemoryStream(data);
            return Task.FromResult<Stream?>(stream);
        }
        return Task.FromResult<Stream?>(null);
    }

    public Task RemoverAsync(string nomeArquivoArmazenamento, CancellationToken cancellationToken)
    {
        _storage.TryRemove(nomeArquivoArmazenamento, out _);
        return Task.CompletedTask;
    }

    public bool EstaDisponivel()
    {
        return true;
    }
}

