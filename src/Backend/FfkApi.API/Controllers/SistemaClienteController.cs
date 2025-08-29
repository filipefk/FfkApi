using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.SistemaCliente.Alterar;
using FfkApi.Application.UseCases.SistemaCliente.Cadastrar;
using FfkApi.Application.UseCases.SistemaCliente.Excluir;
using FfkApi.Application.UseCases.SistemaCliente.Pegar;
using FfkApi.Application.UseCases.SistemaCliente.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
[UsuarioAdministrador]
public sealed class SistemaClienteController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosSistemaCliente), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosSistemaCliente>> Cadastrar(
        [FromServices] ICadastrarSistemaClienteUseCase useCase,
        [FromBody] RequestCadastrarSistemaCliente request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarSistemaClienteUseCase useCase,
        [FromBody] RequestAlterarSistemaCliente request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosSistemaCliente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDadosSistemaCliente>> Pegar(
        [FromServices] IPegarSistemaClienteUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarSistemaCliente
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosSistemaCliente>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosSistemaCliente>>> Pesquisar(
        [FromServices] IPesquisarSistemaClienteUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        [FromServices] IExcluirSistemaClienteUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirSistemaCliente
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
