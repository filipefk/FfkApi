using AutoMapper;
using FfkApi.Application.Validators;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;

namespace FfkApi.Application.UseCases.Feed.Pegar;

public class PegarFeedUseCase : IPegarFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IFeedRepository _feedRepository;

    public PegarFeedUseCase(
        IFeedRepository feedRepository,
        IMapper mapper)
    {
        _feedRepository = feedRepository;
        _mapper = mapper;
    }

    public async Task<ResponseDadosFeed> Execute(RequestPegarFeed request, CancellationToken cancellationToken)
    {
        var idValido = IdValidator.ValidarId(request.Id);

        var feed = await _feedRepository.PegarFeedPorId(idValido, cancellationToken);

        if (feed == null)
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        return _mapper.Map<ResponseDadosFeed>(feed);
    }
}
