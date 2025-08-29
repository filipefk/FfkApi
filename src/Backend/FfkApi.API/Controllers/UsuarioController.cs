using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Usuario.Alterar;
using FfkApi.Application.UseCases.Usuario.AlterarPermissoes;
using FfkApi.Application.UseCases.Usuario.Ativar;
using FfkApi.Application.UseCases.Usuario.Cadastrar;
using FfkApi.Application.UseCases.Usuario.Excluir;
using FfkApi.Application.UseCases.Usuario.NovaSenha;
using FfkApi.Application.UseCases.Usuario.Pegar;
using FfkApi.Application.UseCases.Usuario.Pesquisar;
using FfkApi.Application.UseCases.Usuario.TrocarSenha;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class UsuarioController : ControllerBase
{
    [UsuarioAutenticado(Permissao = "Cadastro de Usuários")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosUsuario), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosUsuario>> Cadastrar(
        [FromServices] ICadastrarUsuarioUseCase useCase,
        [FromBody] RequestCadastrarUsuario request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [UsuarioAutenticado]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseDadosUsuario>> PegarUsuarioPorId(
        [FromServices] IPegarUsuarioPorIdUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarUsuario
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado]
    [HttpGet]
    [ProducesResponseType(typeof(ResponseDadosUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseDadosUsuario>> PegarUsuarioLogado(
        [FromServices] IPegarUsuarioLogadoUseCase useCase,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Usuários")]
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosUsuario>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosUsuario>>> Pesquisar(
        [FromServices] IPesquisarUsuarioUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarUsuarioUseCase useCase,
        [FromBody] RequestAlterarUsuario request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Usuários")]
    [HttpPut("permissoes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AlterarPermissoes(
        [FromServices] IAlterarPermissoesUsuarioUseCase useCase,
        [FromBody] RequestAlterarPermissoesUsuario request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Usuários")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Excluir(
        [FromServices] IExcluirUsuarioUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirUsuario
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAutenticado]
    [HttpPut("trocar-senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TrocarSenha(
        [FromServices] ITrocarSenhaUseCase useCase,
        [FromBody] RequestTrocarSenha request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [HttpPut("ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Ativar(
        [FromServices] IAtivarUsuarioUseCase useCase,
        [FromBody] RequestAtivarUsuario request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [HttpPut("nova-senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> NovaSenha(
        [FromServices] INovaSenhaUsuarioUseCase useCase,
        [FromBody] RequestNovaSenhaUsuario request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
