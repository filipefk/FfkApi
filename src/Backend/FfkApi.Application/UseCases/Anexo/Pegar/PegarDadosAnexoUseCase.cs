using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Anexo.Pegar;

public class PegarDadosAnexoUseCase : IPegarDadosAnexoUseCase
{
    private readonly IMapper _mapper;
    private readonly IAnexoRepository _anexoRepository;

    public PegarDadosAnexoUseCase(
        IAnexoRepository anexoRepository,
        IMapper mapper)
    {
        _anexoRepository = anexoRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDadosAnexo> Execute(RequestPegarDadosAnexo request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var anexo = await _anexoRepository.PegarAnexoPorId(idValido, cancellationToken);

        if (anexo == null)
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        return _mapper.Map<ResponseDadosAnexo>(anexo);
    }
}
