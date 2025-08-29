using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Feed.Pesquisar;

public class PesquisarFeedUseCase : IPesquisarFeedUseCase
{
    private readonly IMapper _mapper;
    private readonly IFeedRepository _feedRepository;

    public PesquisarFeedUseCase(
        IFeedRepository feedRepository,
        IMapper mapper)
    {
        _feedRepository = feedRepository;
        _mapper = mapper;
    }

    public async Task<ResponsePaginado<ResponseDadosFeed>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var quantidadeTotal = await _feedRepository.QuantidadeTotal(cancellationToken);
        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Feed>("Feeds");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Feed), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Feed>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(_feedRepository.AsQueryable()) as IQueryable<Domain.Entities.Feed>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosFeed>(
            _mapper.Map<List<ResponseDadosFeed>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
