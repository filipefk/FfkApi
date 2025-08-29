using FfkApi.Communication.Requests;
using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.InlineData;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Aceitacao.Test.Usuario.AlterarPermissoes;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class E2EAlterarPermissoesUsuarioTest : E2EClassFixture
{
    private readonly string _baseUrlAlterarPermissoes = "usuario/permissoes";

    [Test]
    public async Task Sucesso_Administrador_Com_Perfis_E_Permissoes()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novoUsuario = await CadastroHelper.CadastrarNovoUsuario();

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = novoUsuario.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Administrador_Sem_Perfis(List<string>? perfisAcesso)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novoUsuario = await CadastroHelper.CadastrarNovoUsuario();

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = novoUsuario.Id.ToString(),
            PerfisAcesso = perfisAcesso,
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, TestCaseSource(typeof(ListaStringNulaVaziaInlineData), nameof(ListaStringNulaVaziaInlineData.ListaStringNulaVazia))]
    public async Task Sucesso_Administrador_Sem_Permissoes(List<string>? permissoes)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var novoUsuario = await CadastroHelper.CadastrarNovoUsuario();

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = novoUsuario.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = permissoes
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroUsuarios.Id);

        var novoUsuario = await CadastroHelper.CadastrarNovoUsuario();

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = novoUsuario.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    public async Task Erro_Nenhuma_Alteracao(List<string>? listaVazia)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarPermissoesUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("NENHUMA_ALTERACAO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nenhum_Perfil_Acesso_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = _usuarioSemPerfilNemPermissao.Id.ToString();
        request.Permissoes = null;

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIS_ACESSO_NAO_ENCONTRADOS").Replace("{lista}", request.PerfisAcesso!.ListaSepadadaPorVirgula());

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Algum_Perfil_Acesso_Nao_Encontrados()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            Permissoes = null,
            PerfisAcesso = [_perfilAcessoGerente.Nome, "PerfilInvalido"]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIS_ACESSO_NAO_ENCONTRADOS").Replace("{lista}", "PerfilInvalido");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Perfil_Acesso_Vazio(string? perfilAcesso)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            Permissoes = null,
            PerfisAcesso = [_perfilAcessoGerente.Nome, perfilAcesso!]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERFIL_ACESSO_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Nenhuma_Permissao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarPermissoesUsuarioBuilder.Build();
        request.Id = _usuarioSemPerfilNemPermissao.Id.ToString();
        request.PerfisAcesso = null;

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSOES_NAO_ENCONTRADAS").Replace("{lista}", request.Permissoes!.ListaSepadadaPorVirgula());

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Alguma_Permissao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = null,
            Permissoes = [_permissaoCadastroEquipes.Nome, "PermissaoInvalida"]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSOES_NAO_ENCONTRADAS").Replace("{lista}", "PermissaoInvalida");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("     ")]
    public async Task Erro_Permissao_Vazia(string? permissao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = null,
            Permissoes = [_permissaoCadastroEquipes.Nome, permissao!]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("PERMISSAO_VAZIA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("123")]
    [TestCase("asdfasdf")]
    [TestCase("b9fc55af-e38u-4852-b4f5-ad6b1277472d")]
    public async Task Erro_Id_Invalido(string id)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = id,
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = Guid.NewGuid().ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(_baseUrlAlterarPermissoes, cancellationToken, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_TOKEN");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(_baseUrlAlterarPermissoes, cancellationToken, request, "TokenInvalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_De_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(_baseUrlAlterarPermissoes, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Expirado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2Ns"
                + "YWltcy9zaWQiOiIwNzBhMjYzYy1hODViLTQyOWQtODM5Ny1iYTBjMTZlNjYyOTAiLCJuYmYiOjE3NDU4NDE1MjMsImV4cCI6MTc0NTg0MTU4M"
                + "iwiaWF0IjoxNzQ1ODQxNTIzfQ.zgcOTtirTevb3SgdvDerGUt25TAR079ps0vNIOQHZ4g";

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(_baseUrlAlterarPermissoes, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token, addAppToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_AUSENTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = new RequestAlterarPermissoesUsuario
        {
            Id = _usuarioSemPerfilNemPermissao.Id.ToString(),
            PerfisAcesso = [_perfilAcessoGerente.Nome],
            Permissoes = [_permissaoCadastroEquipes.Nome]
        };

        var response = await HttpHelper.DoPut(url: _baseUrlAlterarPermissoes, cancellationToken: cancellationToken, request: request, token: token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
