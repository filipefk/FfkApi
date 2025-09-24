#if DEBUG
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Security.Credenciais;
using FfkApi.Domain.Security.Criptografia;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class CredenciaisController : ControllerBase
{
    [HttpGet("senha")]
    [ProducesResponseType(typeof(ResponseSenhaValida), StatusCodes.Status200OK)]
    public ActionResult<ResponseSenhaValida> GerarSenhaValida(
        [FromServices] IGeradorSenhaValida gerador)
    {
        var response = new ResponseSenhaValida()
        {
            Senha = gerador.GerarSenha()
        };
        return Ok(response);
    }

    [HttpGet("token")]
    [ProducesResponseType(typeof(ResponseToken), StatusCodes.Status200OK)]
    public ActionResult<ResponseToken> GerarToken(
        [FromServices] IGeradorToken gerador)
    {
        var response = new ResponseToken()
        {
            Token = gerador.GerarToken()
        };
        return Ok(response);
    }

    [HttpPost("encriptar")]
    [ProducesResponseType(typeof(ResponseSenhaEncriptada), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    public ActionResult<ResponseSenhaEncriptada> EncriptarSenha(
        [FromServices] IEncriptadorSenha encriptador,
        [FromBody] RequestEncriptarSenha request)
    {
        var response = new ResponseSenhaEncriptada()
        {
            SenhaEncriptada = encriptador.Encriptar(request.Senha)
        };
        return Ok(response);
    }
}
#endif