using FfkApi.Application.Services.Anexo;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Excluir;

public class ExcluirFeedUseCase : IExcluirFeedUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeedRepository _feedRepository;
    private readonly IArmazenadorDeAnexoService _armazenadorDeAnexoService;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public ExcluirFeedUseCase(
        IFeedRepository feedRepository,
        IUnitOfWork unitOfWork,
        IArmazenadorDeAnexoService armazenadorDeAnexoService,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _feedRepository = feedRepository;
        _unitOfWork = unitOfWork;
        _armazenadorDeAnexoService = armazenadorDeAnexoService;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task Execute(RequestExcluirFeed request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var feed = usuarioLogado.TemPerfilAdministrador() ?
            await _feedRepository.PegarFeedPorId(idValido, cancellationToken) :
            await _feedRepository.PegarFeedPorId(idValido, usuarioLogado.Organizacao.Id, cancellationToken);

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
