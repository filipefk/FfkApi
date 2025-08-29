using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.Arquivos;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Anexo.Pegar;

public class PegarArquivoAnexoUseCase : IPegarArquivoAnexoUseCase
{
    private readonly IAnexoRepository _anexoRepository;
    private readonly IArmazenadorDeArquivoService _armazenadorDeArquivo;

    public PegarArquivoAnexoUseCase(
        IAnexoRepository anexoRepository,
        IArmazenadorDeArquivoService armazenadorDeArquivo)
    {
        _anexoRepository = anexoRepository;
        _armazenadorDeArquivo = armazenadorDeArquivo;
    }

    public async Task<ResponseArquivoAnexo> Execute(RequestPegarArquivoAnexo request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var anexo = await _anexoRepository.PegarAnexoPorId(idValido, cancellationToken);

        if (anexo == null)
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        var stream = await _armazenadorDeArquivo.ObterAsync(anexo.NomeArquivoArmazenamento, cancellationToken);

        if (stream == null)
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        var response = new ResponseArquivoAnexo
        {
            NomeArquivo = anexo.NomeArquivo,
            MimeType = anexo.MimeType,
            stream = stream
        };

        return response;
    }
}
