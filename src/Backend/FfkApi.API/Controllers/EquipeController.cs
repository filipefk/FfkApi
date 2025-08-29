using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Equipe.Alterar;
using FfkApi.Application.UseCases.Equipe.Cadastrar;
using FfkApi.Application.UseCases.Equipe.Excluir;
using FfkApi.Application.UseCases.Equipe.Pegar;
using FfkApi.Application.UseCases.Equipe.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class EquipeController : ControllerBase
{
    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosEquipe), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosEquipe>> Cadastrar(
        [FromServices] ICadastrarEquipeUseCase useCase,
        [FromBody] RequestCadastrarEquipe request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarEquipeUseCase useCase,
        [FromBody] RequestAlterarEquipe request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAutenticado]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosEquipe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDadosEquipe>> Pegar(
        [FromServices] IPegarEquipeUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarEquipe
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado]
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosEquipe>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosEquipe>>> Pesquisar(
        [FromServices] IPesquisarEquipeUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        [FromServices] IExcluirEquipeUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirEquipe
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
