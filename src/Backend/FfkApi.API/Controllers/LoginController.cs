using FfkApi.Application.UseCases.Login.LoginSistema;
using FfkApi.Application.UseCases.Login.LoginUsuario;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class LoginController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseLoginUsuario), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseLoginUsuario>> Login(
        [FromServices] ILoginUsuarioUseCase useCase,
        [FromBody] RequestLoginUsuario request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("sistema")]
    [ProducesResponseType(typeof(ResponseLoginSistemaCliente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseLoginSistemaCliente>> LoginSistema(
        [FromServices] ILoginSistemaClienteUseCase useCase,
        [FromBody] RequestLoginSistemaCliente request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }
}
