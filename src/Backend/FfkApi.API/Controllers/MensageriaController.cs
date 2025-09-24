#if DEBUG
using FfkApi.Communication.Responses;
using FfkApi.Domain.Services.Mensageria;
using FfkApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public class MensageriaController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> EnviarMensagem(
        [FromServices] IPublicarMensagemService publicador,
        [FromBody] object request,
        CancellationToken cancellationToken)
    {
        if (!publicador.EstaDisponivel())
        {
            var response = new ResponseErro(ResourceMessagesException.MENSAGERIA_INDISPONIVEL);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        await publicador.PublicarAsync(request, cancellationToken: cancellationToken);
        return Ok();
    }
}
#endif