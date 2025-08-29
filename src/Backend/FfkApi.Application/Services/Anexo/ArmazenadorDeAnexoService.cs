using AutoMapper;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.Arquivos;

namespace FfkApi.Application.Services.Anexo;

public class ArmazenadorDeAnexoService : IArmazenadorDeAnexoService
{
    private readonly IMapper _mapper;
    private readonly IAnexoRepository _anexoRepository;
    private readonly IArmazenadorDeArquivoService _armazenadorDeArquivo;

    public ArmazenadorDeAnexoService(
        IAnexoRepository anexoRepository,
        IMapper mapper,
        IArmazenadorDeArquivoService armazenadorDeArquivo)
    {
        _anexoRepository = anexoRepository;
        _mapper = mapper;
        _armazenadorDeArquivo = armazenadorDeArquivo;
    }

    public async Task<Domain.Entities.Anexo> SalvarAsync(RequestCadastrarAnexo request, CancellationToken cancellationToken)
    {
        using var stream = request.Arquivo!.OpenReadStream();
        var nomeArquivoArmazenamento = await _armazenadorDeArquivo.SalvarAsync(stream, request.Arquivo.FileName, cancellationToken);

        var anexo = _mapper.Map<Domain.Entities.Anexo>(request);
        anexo.NomeArquivo = request.Arquivo.FileName;
        anexo.NomeArquivoArmazenamento = nomeArquivoArmazenamento;
        anexo.Extensao = Path.GetExtension(request.Arquivo.FileName) ?? string.Empty;
        anexo.TamanhoBytes = request.Arquivo.Length;
        anexo.MimeType = request.Arquivo.ContentType ?? string.Empty;

        await _anexoRepository.Adicionar(anexo, cancellationToken);
        return anexo;
    }

    public async Task ExcluirAsync(Domain.Entities.Anexo anexo, CancellationToken cancellationToken)
    {
        await _anexoRepository.Excluir(anexo.Id, cancellationToken);
        await _armazenadorDeArquivo.RemoverAsync(anexo.NomeArquivoArmazenamento, cancellationToken);
    }
}
