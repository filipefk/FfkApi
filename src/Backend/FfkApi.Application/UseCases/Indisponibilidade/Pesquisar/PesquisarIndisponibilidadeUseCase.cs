using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Domain.Services.UsuarioLogado;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Indisponibilidade.Pesquisar;

public class PesquisarIndisponibilidadeUseCase : IPesquisarIndisponibilidadeUseCase
{
    private readonly IMapper _mapper;
    private readonly IIndisponibilidadeRepository _indisponibilidadeRepository;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PesquisarIndisponibilidadeUseCase(
        IIndisponibilidadeRepository indisponibilidadeRepository,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _indisponibilidadeRepository = indisponibilidadeRepository;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<ResponsePaginado<ResponseDadosIndisponibilidade>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var usuarioLogado = await _usuarioLogadoService.PegarUsuarioLogadoAtivo(cancellationToken);
        var administrador = usuarioLogado.TemPerfilAdministrador();

        var quantidadeTotal = administrador ?
            await _indisponibilidadeRepository.QuantidadeTotal(cancellationToken) :
            await _indisponibilidadeRepository.QuantidadeTotal(usuarioLogado.Organizacao.Id, cancellationToken);

        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA);

        var baseQuery = administrador ?
            _indisponibilidadeRepository.AsQueryable() :
            _indisponibilidadeRepository.AsQueryable()
                .Where(i => i.Usuario.Organizacao.Id == usuarioLogado.Organizacao.Id);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Indisponibilidade>("Indisponibilidades");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Indisponibilidade), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Indisponibilidade>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(baseQuery) as IQueryable<Domain.Entities.Indisponibilidade>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.INDISPONIBILIDADE_NAO_ENCONTRADA);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosIndisponibilidade>(
            _mapper.Map<List<ResponseDadosIndisponibilidade>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
