using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Pegar;

public class PegarFeedUseCase : IPegarFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IFeedRepository _feedRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PegarFeedUseCase(
        IFeedRepository feedRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _feedRepository = feedRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponseDadosFeed> Execute(RequestPegarFeed request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);

        var feed = usuarioLogado.TemPerfilAdministrador() ?
            await _feedRepository.PegarFeedPorId(idValido, cancellationToken) :
            await _feedRepository.PegarFeedPorId(idValido, usuarioLogado.Organizacao.Id, cancellationToken);

        if (feed == null)
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        return _mapper.Map<ResponseDadosFeed>(feed);
    }
}