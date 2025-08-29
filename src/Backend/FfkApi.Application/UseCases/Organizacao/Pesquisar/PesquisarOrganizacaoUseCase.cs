using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Organizacao.Pesquisar;

public class PesquisarOrganizacaoUseCase : IPesquisarOrganizacaoUseCase
{
    private readonly IMapper _mapper;
    private readonly IOrganizacaoRepository _organizacaoRepository;

    public PesquisarOrganizacaoUseCase(
        IOrganizacaoRepository organizacaoRepository,
        IMapper mapper)
    {
        _organizacaoRepository = organizacaoRepository;
        _mapper = mapper;
    }

    public async Task<ResponsePaginado<ResponseDadosOrganizacao>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var quantidadeTotal = await _organizacaoRepository.QuantidadeTotal(cancellationToken);
        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Organizacao>("Organizacoes");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Organizacao), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Organizacao>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(_organizacaoRepository.AsQueryable()) as IQueryable<Domain.Entities.Organizacao>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.ORGANIZACAO_NAO_ENCONTRADA);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosOrganizacao>(
            _mapper.Map<List<ResponseDadosOrganizacao>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
