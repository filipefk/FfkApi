using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Equipe.Pesquisar;

public class PesquisarEquipeUseCase : IPesquisarEquipeUseCase
{
    private readonly IMapper _mapper;
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PesquisarEquipeUseCase(
        IEquipeRepository equipeRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _equipeRepository = equipeRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponsePaginado<ResponseDadosEquipe>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        var administrador = usuarioLogado.TemPerfilAdministrador();

        var quantidadeTotal = administrador ?
            await _equipeRepository.QuantidadeTotal(cancellationToken) :
            await _equipeRepository.QuantidadeTotal(idOrganizacao: usuarioLogado.Organizacao.Id, cancellationToken);

        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA);

        var baseQuery = administrador ?
            _equipeRepository.AsQueryable() :
            _equipeRepository.AsQueryable(idOrganizacao: usuarioLogado.Organizacao.Id);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Equipe>("Equipes");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Equipe), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Equipe>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(baseQuery) as IQueryable<Domain.Entities.Equipe>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.EQUIPE_NAO_ENCONTRADA);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosEquipe>(
            _mapper.Map<List<ResponseDadosEquipe>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
