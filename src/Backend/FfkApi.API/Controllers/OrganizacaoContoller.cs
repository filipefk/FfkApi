using FfkApi.API.Attributes;
using FfkApi.Application.UseCases.Organizacao.Alterar;
using FfkApi.Application.UseCases.Organizacao.Cadastrar;
using FfkApi.Application.UseCases.Organizacao.CadastrarEmLote;
using FfkApi.Application.UseCases.Organizacao.Excluir;
using FfkApi.Application.UseCases.Organizacao.Pegar;
using FfkApi.Application.UseCases.Organizacao.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using FfkApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FfkApi.API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class OrganizacaoController : ControllerBase
{
    [UsuarioAdministrador]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosOrganizacao), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseDadosOrganizacao>> Cadastrar(
        [FromServices] ICadastrarOrganizacaoUseCase useCase,
        [FromBody] RequestCadastrarOrganizacao request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    [SistemaClienteAutenticado]
    [HttpPost("lote")]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResponseCadastrarEmLote<RequestCadastrarOrganizacao, ResponseDadosOrganizacao>>> CadastrarEmLote(
        [FromServices] ICadastrarOrganizacaoEmLoteUseCase useCase,
        [FromBody] RequestCadastrarOrganizacaoEmLote request,
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

    [UsuarioAdministrador]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarOrganizacaoUseCase useCase,
        [FromBody] RequestAlterarOrganizacao request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    [UsuarioAdministrador]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosOrganizacao), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDadosOrganizacao>> Pegar(
        [FromServices] IPegarOrganizacaoUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestPegarOrganizacao
        {
            Id = id
        };
        var response = await useCase.Execute(request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAdministrador]
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosOrganizacao>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosOrganizacao>>> Pesquisar(
        [FromServices] IPesquisarOrganizacaoUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    [UsuarioAdministrador]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        [FromServices] IExcluirOrganizacaoUseCase useCase,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var request = new RequestExcluirOrganizacao
        {
            Id = id
        };
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }
}
