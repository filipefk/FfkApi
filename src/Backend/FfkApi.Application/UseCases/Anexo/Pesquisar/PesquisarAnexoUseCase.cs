using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Anexo.Pesquisar;

public class PesquisarAnexoUseCase : IPesquisarAnexoUseCase
{
    private readonly IMapper _mapper;
    private readonly IAnexoRepository _anexoRepository;

    public PesquisarAnexoUseCase(
        IAnexoRepository anexoRepository,
        IMapper mapper)
    {
        _anexoRepository = anexoRepository;
        _mapper = mapper;
    }

    public async Task<ResponsePaginado<ResponseDadosAnexo>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var quantidadeTotal = await _anexoRepository.QuantidadeTotal(cancellationToken);
        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Anexo>("Anexos");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Anexo), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Anexo>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(_anexoRepository.AsQueryable()) as IQueryable<Domain.Entities.Anexo>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.ANEXO_NAO_ENCONTRADO);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosAnexo>(
            _mapper.Map<List<ResponseDadosAnexo>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
