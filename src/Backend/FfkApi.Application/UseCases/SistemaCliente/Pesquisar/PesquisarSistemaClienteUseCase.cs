using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.SistemaCliente.Pesquisar;

public class PesquisarSistemaClienteUseCase : IPesquisarSistemaClienteUseCase
{
    private readonly IMapper _mapper;
    private readonly ISistemaClienteRepository _sistemaClienteRepository;

    public PesquisarSistemaClienteUseCase(
        ISistemaClienteRepository sistemaClienteRepository,
        IMapper mapper)
    {
        _sistemaClienteRepository = sistemaClienteRepository;
        _mapper = mapper;
    }

    public async Task<ResponsePaginado<ResponseDadosSistemaCliente>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var quantidadeTotal = await _sistemaClienteRepository.QuantidadeTotal(cancellationToken);
        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.SistemaCliente>("SistemasCliente");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.SistemaCliente), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.SistemaCliente>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(_sistemaClienteRepository.AsQueryable()) as IQueryable<Domain.Entities.SistemaCliente>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.SISTEMACLIENTE_NAO_ENCONTRADO);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosSistemaCliente>(
            _mapper.Map<List<ResponseDadosSistemaCliente>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}
