using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.Extension;
using TestUtil.HttpUtil;
using TestUtil.Tokens;

namespace Integracao.Test.Usuario.Pegar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class PegarUsuarioPorIdTest : FfkApiClassFixture
{
    private readonly string _baseUrlUsuario = "usuario";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroUsuarios;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;

    public PegarUsuarioPorIdTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroUsuarios = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroUsuarios"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
    }

    [Test]
    public async Task Sucesso_Pegar_Voce_Mesmo()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(_usuarioSemPerfilNemPermissao.Id.ToString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioSemPerfilNemPermissao.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(_usuarioSemPerfilNemPermissao.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(_usuarioSemPerfilNemPermissao.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(_usuarioSemPerfilNemPermissao.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(_usuarioSemPerfilNemPermissao.Status.ToString()));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioSemPerfilNemPermissao.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.PerfisAcesso.ToListNome()!));

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Sucesso_Administrador()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(_usuarioSemPerfilNemPermissao.Id.ToString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioSemPerfilNemPermissao.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(_usuarioSemPerfilNemPermissao.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(_usuarioSemPerfilNemPermissao.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(_usuarioSemPerfilNemPermissao.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(_usuarioSemPerfilNemPermissao.Status.ToString()));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioSemPerfilNemPermissao.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.PerfisAcesso.ToListNome()!));

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Sucesso_Usuario_Com_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroUsuarios.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var id = dadosDaResposta.RootElement.GetProperty("id").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(id));
        Assert.That(id, Is.EqualTo(_usuarioSemPerfilNemPermissao.Id.ToString()));

        var nome = dadosDaResposta.RootElement.GetProperty("nome").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(nome));
        Assert.That(nome, Is.EqualTo(_usuarioSemPerfilNemPermissao.Nome));

        var email = dadosDaResposta.RootElement.GetProperty("email").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(email));
        Assert.That(email, Is.EqualTo(_usuarioSemPerfilNemPermissao.Email));

        var cpf = dadosDaResposta.RootElement.GetProperty("cpf").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(cpf));
        Assert.That(cpf, Is.EqualTo(_usuarioSemPerfilNemPermissao.Cpf));

        var telefone = dadosDaResposta.RootElement.GetProperty("telefone").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(telefone));
        Assert.That(telefone, Is.EqualTo(_usuarioSemPerfilNemPermissao.Telefone));

        var status = dadosDaResposta.RootElement.GetProperty("status").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(status));
        Assert.That(status, Is.EqualTo(_usuarioSemPerfilNemPermissao.Status.ToString()));

        var organizacao = dadosDaResposta.RootElement.GetProperty("organizacao").GetString();
        Assert.That(!string.IsNullOrWhiteSpace(organizacao));
        Assert.That(organizacao, Is.EqualTo(_usuarioSemPerfilNemPermissao.Organizacao.Nome));

        var perfisAcesso = dadosDaResposta.RootElement.GetProperty("perfisAcesso").EnumerateArray();
        Assert.That(perfisAcesso, Is.Not.Null);
        Assert.That(perfisAcesso.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.PerfisAcesso.ToListNome()!));

        var permissoes = dadosDaResposta.RootElement.GetProperty("permissoes").EnumerateArray();
        Assert.That(permissoes, Is.Not.Null);
        Assert.That(permissoes.ToListString(), Is.EquivalentTo(_usuarioSemPerfilNemPermissao.Permissoes.ToListNome()!));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlUsuario}/{_usuarioAdministrador.Id}", cancellationToken: cancellationToken, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Usuários");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var response = await HttpHelper.DoGet(url: $"{_baseUrlUsuario}/{Guid.NewGuid()}", cancellationToken: cancellationToken, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("USUARIO_NAO_ENCONTRADO");

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

        var response = await HttpHelper.DoGet(url: $"{_baseUrlUsuario}/{id}", cancellationToken: cancellationToken, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ID_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_Invalido()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, "tokenInvalid");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Sem_Token()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_TOKEN");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Token_De_Usuario_Nao_Encontrado()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(Guid.NewGuid());

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token);

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

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token);

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token, addAppToken: false);

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

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var response = await HttpHelper.DoGet($"{_baseUrlUsuario}/{_usuarioSemPerfilNemPermissao.Id}", cancellationToken, token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
