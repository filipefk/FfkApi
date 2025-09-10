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
[Produces("application/json")]
public sealed class OrganizacaoController : ControllerBase
{
    /// <summary>
    /// /organizacao - Cadastrar organização
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja cadastrar uma nova organização.
    /// </remarks>
    /// <response code="201">Organização criada com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /organizacao/lote - Cadastrar organizações em lote
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para cadastrar múltiplas organizações de uma só vez.
    /// </remarks>
    /// <response code="201">Todas as organizações foram cadastradas com sucesso</response>
    /// <response code="200">Algumas organizações foram cadastradas com sucesso, outras falharam</response>
    /// <response code="400">Falha ao cadastrar as organizações ou erro de validação dos dados</response>
    /// <response code="401">Erro de validação do token do sistema cliente</response>
    /// <response code="403">Sistema Cliente sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /organizacao - Alterar organização
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para alterar os dados de uma organização existente.
    /// </remarks>
    /// <response code="204">Organização alterada com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /organizacao/{id} - Consultar organização
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para consultar os dados de uma organização pelo seu identificador.
    /// </remarks>
    /// <response code="200">Organização encontrada com sucesso</response>
    /// <response code="400">Identificador inválido</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="404">Organização não encontrada</response>
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

    /// <summary>
    /// /organizacao/pesquisar - Pesquisar organizações
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para pesquisar organizações utilizando filtros e paginação.
    /// </remarks>
    /// <response code="200">Pesquisa realizada com sucesso</response>
    /// <response code="500">Erro interno ao realizar a pesquisa</response>
    /// <response code="404">Nenhuma organização encontrada</response>
    /// <response code="401">Erro de validação do token do usuário</response>
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

    /// <summary>
    /// /organizacao/{id} - Excluir organização
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para excluir uma organização pelo seu identificador.
    /// </remarks>
    /// <response code="204">Organização excluída com sucesso</response>
    /// <response code="400">Identificador inválido</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    /// <response code="404">Organização não encontrada</response>
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
