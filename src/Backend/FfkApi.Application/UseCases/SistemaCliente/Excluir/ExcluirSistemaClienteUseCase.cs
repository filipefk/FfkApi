using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.SistemaCliente.Excluir;

public class ExcluirSistemaClienteUseCase : IExcluirSistemaClienteUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;

    public ExcluirSistemaClienteUseCase(
        ISistemaClienteRepository sistemaClienteRepository,
        IUnitOfWork unitOfWork)
    {
        _sistemaClienteRepository = sistemaClienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestExcluirSistemaCliente request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var sistemaCliente = await _sistemaClienteRepository.PegarSistemaClientePorId(idValido, cancellationToken);

        if (sistemaCliente == null)
            throw new NotFoundException(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO);

        await _sistemaClienteRepository.Excluir(idValido, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
