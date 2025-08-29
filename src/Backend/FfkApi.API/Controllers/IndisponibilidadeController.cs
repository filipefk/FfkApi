using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Indisponibilidade.Alterar;
using FfkApi.Application.UseCases.Indisponibilidade.Cadastrar;
using FfkApi.Application.UseCases.Indisponibilidade.Excluir;
using FfkApi.Application.UseCases.Indisponibilidade.Pegar;
using FfkApi.Application.UseCases.Indisponibilidade.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
[UsuarioAutenticado]
public sealed class IndisponibilidadeController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosIndisponibilidade), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosIndisponibilidade>> Cadastrar(
        [FromServices] ICadastrarIndisponibilidadeUseCase useCase,
        [FromBody] RequestCadastrarIndisponibilidade request,
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
        [FromServices] IAlterarIndisponibilidadeUseCase useCase,
        [FromBody] RequestAlterarIndisponibilidade request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosIndisponibilidade), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDadosIndisponibilidade>> Pegar(
        [FromServices] IPegarIndisponibilidadeUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarIndisponibilidade
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosIndisponibilidade>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosIndisponibilidade>>> Pesquisar(
        [FromServices] IPesquisarIndisponibilidadeUseCase useCase,
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
        [FromServices] IExcluirIndisponibilidadeUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirIndisponibilidade
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
