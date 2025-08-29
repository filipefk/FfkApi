using FfkApi.Application.Services.Anexo;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Excluir;

public class ExcluirFeedUseCase : IExcluirFeedUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedRepository _feedRepository;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;

    public ExcluirFeedUseCase(
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        IArmazenadorDeAnexoService armazenadorDeAnexoService)
    {
        _feedRepository = feedRepository;
        _unitOfWork = unitOfWork;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
    }

    public async Task Execute(RequestExcluirFeed request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var feed = await _feedRepository.PegarFeedPorId(idValido, cancellationToken);

        if (feed == null)
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        foreach (var anexo in feed.Anexos)
        {
            await _armazenadorDeAnexoService.ExcluirAsync(anexo, cancellationToken);
        }

        await _feedRepository.Excluir(idValido, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
