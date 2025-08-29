using FfkApi.Domain.Services.Arquivos;

namespace FfkApi.Infrastructure.Services.Arquivos;

public class ArmazenadorDeArquivoLocalService : IArmazenadorDeArquivoService
{
    private readonly string _caminhoBase;

    public ArmazenadorDeArquivoLocalService()
    {
        _caminhoBase = "anexos";
        Directory.CreateDirectory(_caminhoBase);
    }

    public async Task<string> SalvarAsync(Stream arquivoStream, string nomeArquivo, CancellationToken cancellationToken)
    {
        var nomeArquivoArmazenamento = $"{Guid.NewGuid()}{Path.GetExtension(nomeArquivo)}";
        var caminhoFisico = Path.Combine(_caminhoBase, nomeArquivoArmazenamento);

        using var fileStream = new FileStream(caminhoFisico, FileMode.Create);
        await arquivoStream.CopyToAsync(fileStream, cancellationToken);

        return nomeArquivoArmazenamento;
    }

    public async Task<Stream?> ObterAsync(string nomeArquivoArmazenamento, CancellationToken _)
    {
        var caminhoFisico = Path.Combine(_caminhoBase, nomeArquivoArmazenamento);
        if (!File.Exists(caminhoFisico))
            return null;

        var stream = new FileStream(caminhoFisico, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await Task.FromResult(stream);
    }

    public async Task RemoverAsync(string nomeArquivoArmazenamento, CancellationToken _)
    {
        var caminhoFisico = Path.Combine(_caminhoBase, nomeArquivoArmazenamento);
        if (File.Exists(caminhoFisico))
            File.Delete(caminhoFisico);

        await Task.CompletedTask;
    }

    public bool EstaDisponivel()
    {
        return true;
    }
}
