using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Token;
using FfkApi.Application.UseCases.Token.TokenAtivacao;
using FfkApi.Application.UseCases.Token.TokenNovaSenha;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class TokenController : ControllerBase
{
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ResponseTokens), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseTokens>> NovoTokenUsuario(
        [FromServices] IRefreshTokenUseCase useCase,
        [FromBody] RequestNovoTokenUsuario request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("nova-senha")]
    [ProducesResponseType(typeof(ResponseNomeUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NovoTokenNovaSenha(
        [FromServices] INovoTokenNovaSenhaUseCase useCase,
        [FromBody] RequestNovoTokenNovaSenha request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("ativacao/{tokenAtivacao}")]
    [ProducesResponseType(typeof(ResponseNomeUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseNomeUsuario>> PegarUsuarioPorTokenAtivacao(
        [FromServices] IPegarUsuarioPorTokenAtivacaoUseCase useCase,
        [FromRoute] string tokenAtivacao,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarUsuarioPorTokenAtivacao
        {
            TokenAtivacao = tokenAtivacao
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("nova-senha/{tokenNovaSenha}")]
    [ProducesResponseType(typeof(ResponseNomeUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseNomeUsuario>> PegarUsuarioPorTokenNovaSenha(
        [FromServices] IPegarUsuarioPorTokenNovaSenhaUseCase useCase,
        [FromRoute] string tokenNovaSenha,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarUsuarioPorTokenNovaSenha
        {
            TokenNovaSenha = tokenNovaSenha
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAutenticado(Permissao = "Cadastro de Usuários")]
    [HttpPut("ativacao")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RenovarTokenAtivacao(
        [FromServices] IRenovarTokenAtivacaoUseCase useCase,
        [FromBody] RequestRenovarTokenAtivacao request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

}
