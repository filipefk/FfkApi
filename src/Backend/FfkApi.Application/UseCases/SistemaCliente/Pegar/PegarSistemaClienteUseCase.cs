using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.SistemaCliente.Pegar;

public class PegarSistemaClienteUseCase : IPegarSistemaClienteUseCase
{
    private readonly IMapper _mapper;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;

    public PegarSistemaClienteUseCase(
        ISistemaClienteRepository sistemaClienteRepository,
        IMapper mapper)
    {
        _sistemaClienteRepository = sistemaClienteRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDadosSistemaCliente> Execute(RequestPegarSistemaCliente request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var sistemaCliente = await _sistemaClienteRepository.PegarSistemaClientePorId(idValido, cancellationToken);

        if (sistemaCliente == null)
            throw new NotFoundException(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO);

        return _mapper.Map<ResponseDadosSistemaCliente>(sistemaCliente);
    }
}
