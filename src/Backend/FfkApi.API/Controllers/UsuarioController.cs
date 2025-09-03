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
[Produces("application/json")]
public sealed class UsuarioController : ControllerBase
{
    /// <summary>
    /// /usuario - Cadastrar usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja cadastrar um novo usuário.
    /// 
    /// Exemplo de request:
    ///
    ///     POST /usuario
    ///     {
    ///        "nome": "João Silva",
    ///        "email": "joao@email.com",
    ///        "cpf": "123.456.789-00",
    ///        "telefone": "+55 11 91234-5678",
    ///        "organizacao": "Banco de Desenvolvimento Econômico",
    ///        "perfisAcesso": ["Administrador", "Usuário"],
    ///        "permissoes": ["Cadastro de Usuários", "Visualizar Relatórios"]
    ///     }
    ///
    /// Exemplo de response (201 Created):
    /// 
    ///      {
    ///         "id": "f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c",
    ///         "nome": "João Silva",
    ///         "email": "joao@email.com",
    ///         "cpf": "123.456.789-00",
    ///         "telefone": "+55 11 91234-5678",
    ///         "organizacao": "Banco de Desenvolvimento Econômico",
    ///         "perfisAcesso": ["Administrador", "Usuário"],
    ///         "permissoes": ["Cadastro de Usuários", "Visualizar Relatórios"]
    ///     }
    /// 
    /// Exemplo de response (400 Bad Request):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Já existe um usuário cadastrado com este e-mail.",
    ///          "O Cpf não é válido."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token expirado."
    ///        ],
    ///        "tokenEstaExpirado": true
    ///      }
    ///  
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /usuario - Pegar Usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja pegar os dados de um usuário existente.
    /// 
    /// Exemplo de request:
    ///
    ///     GET /usuario/f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c
    ///
    /// Exemplo de response (200 OK):
    /// 
    ///      {
    ///         "id": "f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c",
    ///         "nome": "João Silva",
    ///         "email": "joao@email.com",
    ///         "cpf": "123.456.789-00",
    ///         "telefone": "+55 11 91234-5678",
    ///         "organizacao": "Banco de Desenvolvimento Econômico",
    ///         "perfisAcesso": ["Administrador", "Usuário"],
    ///         "permissoes": ["Cadastro de Usuários", "Visualizar Relatórios"]
    ///     }
    /// 
    /// Exemplo de response (400 Bad Request):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "ID Inválido"
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// 
    /// Exemplo de response (404 Not Found):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Usuario não encontrado."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token inválido."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="200">Dados obtidos com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    /// <response code="404">Usuário não encontrado</response>
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

    /// <summary>
    /// /usuario - Pegar Usuário Logado
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para pegar os dados do usuário logado.
    /// 
    /// Exemplo de request:
    ///
    ///     GET /usuario
    ///
    /// Exemplo de response (200 OK):
    /// 
    ///      {
    ///         "id": "f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c",
    ///         "nome": "João Silva",
    ///         "email": "joao@email.com",
    ///         "cpf": "123.456.789-00",
    ///         "telefone": "+55 11 91234-5678",
    ///         "organizacao": "Banco de Desenvolvimento Econômico",
    ///         "perfisAcesso": ["Administrador", "Usuário"],
    ///         "permissoes": ["Cadastro de Usuários", "Visualizar Relatórios"]
    ///     }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Nenhum token encontrado na requisição."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="200">Dados obtidos com sucesso</response>
    /// <response code="401">Erro de validação do token do usuário</response>
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

    /// <summary>
    /// /usuario - Pesquisar Usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja pesquisar os dados dos usuários cadastrados. Se utiliza de query OData para filtros, paginação e ordenação.
    /// 
    /// Exemplo de request:
    ///
    ///     GET /usuario/pesquisar/Filter=contains(Nome, 'Maria') and status eq 'Ativo'&amp;OrderBy=Nome desc&amp;Top=2&amp;Skip=0
    ///
    /// Exemplo de response (200 OK):
    /// 
    ///     {
    ///         "resultados": [
    ///             {
    ///                 "id": "ddd87cb7-d4c8-4d7e-a2df-e45121e7b838",
    ///                 "nome": "Maria Madalena",
    ///                 "email": "Maria.Madalena@hotmail.com",
    ///                 "cpf": "65852007722",
    ///                 "telefone": "+5547985694276",
    ///                 "status": "Ativo",
    ///                 "organizacao": "Banco Inter"
    ///             },
    ///             {
    ///                 "id": "17ebf234-ff28-4a1a-98a1-24d25ed72215",
    ///                 "nome": "Maria das Dores",
    ///                 "email": "Maria.Dores@hotmail.com",
    ///                 "cpf": "02505111693",
    ///                 "telefone": "+5571995474661",
    ///                 "status": "Ativo",
    ///                 "organizacao": "Academia Golias"
    ///             }
    ///         ],
    ///         "paginaAtual": 2,
    ///         "totalDePaginas": 10,
    ///         "tamanhoDaPagina": 2,
    ///         "quantidadeTotal": 19
    ///     }
    /// 
    /// Exemplo de response (500 Internal Server Error):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Erro desconhecido."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// 
    /// Exemplo de response (404 Not Found):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Usuario não encontrado."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token inválido."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="200">Dados obtidos com sucesso</response>
    /// <response code="500">Erro interno. Ocorre também quando a query OData está mau formada</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    /// <response code="404">Usuário não encontrado</response>
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

    /// <summary>
    /// /usuario - Alterar usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja alterar os dados de um usuário existente.
    /// 
    /// Exemplo de request:
    ///
    ///     PUT /usuario
    ///     {
    ///        "id": "f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c",
    ///        "nome": "João Silva",
    ///        "email": "joao@email.com",
    ///        "cpf": "123.456.789-00",
    ///        "telefone": "+55 11 91234-5678",
    ///        "organizacao": "Banco de Desenvolvimento Econômico",
    ///        "perfisAcesso": ["Administrador", "Usuário"],
    ///        "permissoes": ["Cadastro de Usuários", "Visualizar Relatórios"]
    ///     }
    /// 
    /// Exemplo de response (400 Bad Request):
    /// 
    ///      {
    ///          "mensagensDeErro": [
    ///              "A permissão não pode ser vazia",
    ///              "Usuario não encontrado",
    ///              "Perfis de acesso não encontrados: asdfasdf"
    ///          ],
    ///          "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token expirado."
    ///        ],
    ///        "tokenEstaExpirado": true
    ///      }
    ///  
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="204">Usuário alterado com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /usuario - Alterar permissões
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado quando se deseja alterar perfil de acesso e permissões de um usuário existente.
    /// 
    /// Exemplo de request:
    ///
    ///     PUT /usuario/permissoes
    ///     {
    ///         "id": "c8711dc5-a6dc-4cc3-8e5e-08d8b03a5127",
    ///         "perfisAcesso": [
    ///             "Gerente"
    ///         ],
    ///         "permissoes": [
    ///             "Cadastro de Feed"
    ///         ]
    ///     }
    /// 
    /// Exemplo de response (400 Bad Request):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "A permissão não pode ser vazia",
    ///             "Usuario não encontrado",
    ///             "Perfis de acesso não encontrados: Chefão, Dono"
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    ///      
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token expirado."
    ///        ],
    ///        "tokenEstaExpirado": true
    ///      }
    ///  
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="204">Permissões do usuário alteradas com sucesso</response>
    /// <response code="400">Erro de validação nos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
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

    /// <summary>
    /// /usuario - Excluir Usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para excluir os dados de um usuário.
    /// 
    /// Exemplo de request:
    ///
    ///     DELETE /usuario/f3a9d0d5-82b9-4c9a-a93f-8f2b5d8f9b1c
    ///      
    /// Exemplo de response (400 Bad Request):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "ID Inválido"
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    ///     
    /// Exemplo de response (404 Not Found):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Usuario não encontrado."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    ///      
    /// Exemplo de response (403 Forbidden):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "É necessário ter a permissão 'Cadastro de Usuarios' para esta operação."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// 
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Nenhum token encontrado na requisição."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="204">Usuário excluído com sucesso</response>
    /// <response code="400">Erro na validação dos dados enviados</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="403">Usuário sem permissão para executar esta operação</response>
    /// <response code="401">Erro de validação do token do usuário</response>
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

    /// <summary>
    /// /usuario - Trocar Senha
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para o usuário poder trocar a própria senha.
    /// 
    /// Exemplo de request:
    ///
    ///     PUT /usuario/trocar-senha
    ///     {
    ///       "senhaAntiga": "Senha.valida1",
    ///       "novaSenha": "Senha.valida2"
    ///     }
    ///      
    /// Exemplo de response (400 Bad Request):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "A senha não pode ser vazia, deve ter no mínimo 8 caracteres, deve conter pelo menos uma letra maiúscula, uma letra minúscula, um número e um símbolo"
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    /// 
    /// Exemplo de response (401 Unauthorized):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Nenhum token encontrado na requisição."
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// </remarks>
    /// <response code="204">Senha do usuário alterada com sucesso</response>
    /// <response code="400">Erro na validação dos dados enviados</response>
    /// <response code="401">Erro de validação do token do usuário</response>
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

    /// <summary>
    /// /usuario - Ativar Usuário
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para ativar o usuário que foi cadastrado.
    /// 
    /// Exemplo de request:
    ///
    ///     PUT /usuario/ativar
    ///     {
    ///       "tokenAtivacao": "19Nn4ka3NgdeEbfC9WzlyvJnjiOaGUWayYi3OuNKeyE",
    ///       "nome": "John",
    ///       "email": "John_Lebsack49@gmail.com",
    ///       "cpf": "31814475281",
    ///       "senha": "Senha.valida1"
    ///     }
    ///      
    /// Exemplo de response (400 Bad Request):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "Dados inválidos"
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    ///     
    /// Exemplo de response (404 Not Found):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token de ativação não encontrado"
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// 
    /// Exemplo de response (403 Forbidden):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "Token de ativação expirado. Entre em contato com o administrador do sistema e solicite um novo token."
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    /// </remarks>
    /// <response code="204">Usuário ativado com sucesso</response>
    /// <response code="400">Erro na validação dos dados enviados</response>
    /// <response code="404">Token de ativação não encontrado.</response>
    /// <response code="403">Token de ativação expirado</response>
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

    /// <summary>
    /// /usuario - Nova senha
    /// </summary>
    /// <remarks>
    /// Endpoint utilizado para alterar a senha do usuário pelo recurso 'esqueci minha senha'.
    /// 
    /// Exemplo de request:
    ///
    ///     PUT /usuario/nova-senha
    ///     {
    ///       "tokenNovaSenha": "62caaGgxgpOtgKgAt4zvZgiRrhPaNCWcfu5bdFLSBVE",
    ///       "nome": "Beth",
    ///       "email": "Beth4@gmail.com",
    ///       "cpf": "49203935983",
    ///       "novaSenha": "Senha.valida1"
    ///     }
    ///      
    /// Exemplo de response (400 Bad Request):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "Dados inválidos"
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    ///     
    /// Exemplo de response (404 Not Found):
    /// 
    ///      {
    ///        "mensagensDeErro": [
    ///          "Token de senha não encontrado"
    ///        ],
    ///        "tokenEstaExpirado": false
    ///      }
    /// 
    /// Exemplo de response (403 Forbidden):
    /// 
    ///     {
    ///         "mensagensDeErro": [
    ///             "Token de senha expirado. Entre em contato com o administrador do sistema e solicite um novo token."
    ///         ],
    ///         "tokenEstaExpirado": false
    ///     }
    /// </remarks>
    /// <response code="204">Senha alterada com sucesso</response>
    /// <response code="400">Erro na validação dos dados enviados</response>
    /// <response code="404">Token de senha não encontrado.</response>
    /// <response code="403">Token de senha expirado</response>
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
