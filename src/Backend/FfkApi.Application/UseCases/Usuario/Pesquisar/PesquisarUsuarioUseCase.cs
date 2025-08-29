using AutoMapper;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Repositories;
using FfkApi.Exceptions;
using FfkApi.Exceptions.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;

namespace FfkApi.Application.UseCases.Usuario.Pesquisar;

public class PesquisarUsuarioUseCase : IPesquisarUsuarioUseCase
{
    private readonly IMapper _mapper;
    private readonly IUsuarioRepository _usuarioRepository;

    public PesquisarUsuarioUseCase(
        IMapper mapper,
        IUsuarioRepository usuarioRepository)
    {
        _mapper = mapper;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ResponsePaginado<ResponseDadosUsuario>> Execute(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        var quantidadeTotal = await _usuarioRepository.QuantidadeTotal(cancellationToken);
        if (quantidadeTotal == 0)
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<Domain.Entities.Usuario>("Usuarios");
        var odataQueryContext = new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(Domain.Entities.Usuario), new Microsoft.OData.UriParser.ODataPath());
        var queryOptions = new ODataQueryOptions<Domain.Entities.Usuario>(odataQueryContext, httpRequest);
        var resultados = queryOptions.ApplyTo(_usuarioRepository.AsQueryable()) as IQueryable<Domain.Entities.Usuario>;

        if (resultados == null || !resultados.Any())
            throw new NotFoundException(ResourceMessagesException.USUARIO_NAO_ENCONTRADO);

        var tamanhoPagina = (int)quantidadeTotal;
        if (queryOptions.Top != null && queryOptions.Top.Value < (int)quantidadeTotal)
            tamanhoPagina = queryOptions.Top.Value;

        var paginaAtual = 1;
        if (queryOptions.Skip != null && tamanhoPagina < (int)quantidadeTotal)
            paginaAtual = queryOptions.Skip.Value / tamanhoPagina + 1;

        return new ResponsePaginado<ResponseDadosUsuario>(
            _mapper.Map<List<ResponseDadosUsuario>>(resultados!.ToList()),
            paginaAtual,
            tamanhoPagina,
            quantidadeTotal);
    }
}