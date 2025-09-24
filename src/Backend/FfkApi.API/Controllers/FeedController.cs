using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Feed.AdicionarAnexo;
using FfkApi.Application.UseCases.Feed.Alterar;
using FfkApi.Application.UseCases.Feed.Cadastrar;
using FfkApi.Application.UseCases.Feed.CadastrarEmLote;
using FfkApi.Application.UseCases.Feed.Excluir;
using FfkApi.Application.UseCases.Feed.Pegar;
using FfkApi.Application.UseCases.Feed.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class FeedController : ControllerBase
{
    [UsuarioAutenticado(Permissao = "Cadastro de Feeds")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosFeed), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosFeed>> Cadastrar(
        [FromServices] ICadastrarFeedUseCase useCase,
        [FromBody] RequestCadastrarFeed request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Feeds")]
    [HttpPost("anexo")]
    [ProducesResponseType(typeof(ResponseDadosAnexo), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosAnexo>> AdicionarAnexo(
        [FromServices] IAdicionarAnexoFeedUseCase useCase,
        [FromForm] RequestAdicionarAnexoFeed request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [SistemaClienteAutenticado]
    [HttpPost("lote")]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseCadastrarEmLote<RequestCadastrarFeed, ResponseDadosFeed>>> CadastrarEmLote(
        [FromServices] ICadastrarFeedEmLoteUseCase useCase,
        [FromBody] RequestCadastrarFeedEmLote request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        var resultado = EnumUtil.ConverterTextoParaEnum<StatusCadastroLote>(response.Resultado);

        switch (resultado)
        {
            case StatusCadastroLote.SucessoTotal:
                return Created(string.Empty, response);
            case StatusCadastroLote.SucessoParcial:
                return Ok(response);
            case StatusCadastroLote.Indefinido:
            case StatusCadastroLote.Falha:
            default:
                return BadRequest(response);
        }
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Feeds")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarFeedUseCase useCase,
        [FromBody] RequestAlterarFeed request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAutenticado]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosFeed), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDadosFeed>> Pegar(
        [FromServices] IPegarFeedUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarFeed
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado]
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosFeed>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosFeed>>> Pesquisar(
        [FromServices] IPesquisarFeedUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Feeds")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        [FromServices] IExcluirFeedUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirFeed
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
