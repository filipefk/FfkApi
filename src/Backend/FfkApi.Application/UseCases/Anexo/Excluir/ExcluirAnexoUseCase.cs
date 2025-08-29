using FfkApi.Application.Services.Anexo;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Anexo.Excluir;

public class ExcluirAnexoUseCase : IExcluirAnexoUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAnexoRepository _anexoRepository;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;

    public ExcluirAnexoUseCase(
        IAnexoRepository anexoRepository,
        IUnitOfWork unitOfWork,
        IArmazenadorDeAnexoService armazenadorDeAnexoService)
    {
        _anexoRepository = anexoRepository;
        _unitOfWork = unitOfWork;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
    }

    public async Task Execute(RequestExcluirAnexo request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var anexo = await _anexoRepository.PegarAnexoPorId(idValido, cancellationToken);

        if (anexo == null)
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        await _armazenadorDeAnexoService.ExcluirAsync(anexo, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
