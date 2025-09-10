using FfkApi.API.Attributes;
using FfkApi.API.Documentation.Examples;
using FfkApi.Application.UseCases.Equipe.Alterar;
using FfkApi.Application.UseCases.Equipe.Cadastrar;
using FfkApi.Application.UseCases.Equipe.Excluir;
using FfkApi.Application.UseCases.Equipe.Pegar;
using FfkApi.Application.UseCases.Equipe.Pesquisar;
using FfkApi.Communication.Requests;
using FfkApi.Communication.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class EquipeController : ControllerBase
{
    /// <summary>
    /// /equipe - Cadastrar Equipe
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja criar uma nova Equipe dentro de uma Organização
    /// </remarks>
    /// <response code="201">Equipe criada com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseDadosEquipe), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [SwaggerRequestExample(typeof(RequestCadastrarEquipe), typeof(RequestCadastrarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(Response201CreatedCadastrarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ResponseErro400BadRequestCadastrarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(ResponseErro401UnauthorizedExample))]
    [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ResponseErro403ForbiddenExample))]
    public async Task<ActionResult<ResponseDadosEquipe>> Cadastrar(
        [FromServices] ICadastrarEquipeUseCase useCase,
        [FromBody] RequestCadastrarEquipe request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(request, cancellationToken);
        return Created(string.Empty, response);
    }

    /// <summary>
    /// /equipe - Alterar Equipe
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja alterar os dados de uma Equipe existente
    /// </remarks>
    /// <response code="204">Equipe alterada com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [SwaggerRequestExample(typeof(RequestAlterarEquipe), typeof(RequestAlterarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ResponseErro400BadRequestAlterarPegarExample))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(ResponseErro401UnauthorizedExample))]
    [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ResponseErro403ForbiddenExample))]
    public async Task<IActionResult> Alterar(
        [FromServices] IAlterarEquipeUseCase useCase,
        [FromBody] RequestAlterarEquipe request,
        CancellationToken cancellationToken)
    {
        await useCase.Execute(request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// /equipe - Pegar Equipe
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja pegar os dados de uma Equipe existente
    /// </remarks>
    /// <response code="200">Dados obtidos com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="404">Equipe não encontrada</response>
    [UsuarioAutenticado]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseDadosEquipe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Response200OKPegarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ResponseErro400BadRequestAlterarPegarExample))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(ResponseErro401UnauthorizedExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ResponseErro404NotFoundExample))]
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

    /// <summary>
    /// /equipe - Pesquisar Equipe
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se pesquisar uma Equipe existente. Funciona com OData
    /// </remarks>
    /// <response code="200">Pesquisa realizada com sucesso</response>
    /// <response code="500">Erro interno ao realizar a pesquisa</response>
    /// <response code="404">Nenhuma equipe encontrada</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    [UsuarioAutenticado]
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResponsePaginado<ResponseDadosEquipe>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(Response200OKPesquisarEquipeExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ResponseErro500InternalServerErrorExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ResponseErro404NotFoundExample))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(ResponseErro401UnauthorizedExample))]
    public async Task<ActionResult<ResponsePaginado<ResponseDadosEquipe>>> Pesquisar(
        [FromServices] IPesquisarEquipeUseCase useCase,
        [FromQuery] RequestODataQueryOptions _,
        CancellationToken cancellationToken)
    {
        var response = await useCase.Execute(HttpContext.Request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// /equipe - Excluir Equipe
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja excluir os dados de uma Equipe existente
    /// </remarks>
    /// <response code="204">Equipe excluída com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    /// <response code="404">Equipe não encontrada</response>
    [UsuarioAutenticado(Permissao = "Cadastro de Equipes")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseErro), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ResponseErro400BadRequestAlterarPegarExample))]
    [SwaggerResponseExample(StatusCodes.Status401Unauthorized, typeof(ResponseErro401UnauthorizedExample))]
    [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ResponseErro403ForbiddenExample))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ResponseErro404NotFoundExample))]
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