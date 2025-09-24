using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
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
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PesquisarFeedUseCase(
        IFeedRepository feedRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _feedRepository = feedRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponsePaginado<ResponseDadosFeed>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        var administrador = usuarioLogado.TemPerfilAdministrador();

        var quantidadeTotal = administrador ?
            await _feedRepository.QuantidadeTotal(cancellationToken) :
            await _feedRepository.QuantidadeTotal(idOrganizacao: usuarioLogado.Organizacao.Id, cancellationToken);

        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.FEED_NAO_ENCONTRADO);

        var baseQuery = administrador ?
            _feedRepository.AsQueryable() :
            _feedRepository.AsQueryable(idOrganizacao: usuarioLogado.Organizacao.Id);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Feed>("Feeds");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Feed), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Feed>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(baseQuery) as IQueryable<Domain.Entities.Feed>;

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