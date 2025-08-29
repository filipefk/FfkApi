using FfkApi.Domain.Extension;
using FfkApi.Exceptions;
using Integracao.Test.InfraestruturaEmMemoria;
using NUnit.Framework;
using System.Net;
using TestUtil.HttpUtil;
using TestUtil.InlineData;
using TestUtil.Requests;
using TestUtil.Tokens;

namespace Integracao.Test.Usuario.Alterar;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AlterarUsuarioTest : FfkApiClassFixture
{
    private readonly string _baseUrlUsuario = "usuario";
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAdministrador;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioPermissaoCadastroUsuarios;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioSemPerfilNemPermissao;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAlterarStatus;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioAlterarSeusDados;
    private readonly FfkApi.Domain.Entities.Usuario _usuarioTrocarOrganizacao;

    public AlterarUsuarioTest()
    {
        var entidades = _factory.EntidadesCriadas();
        _usuarioAdministrador = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAdministrador"];
        _usuarioPermissaoCadastroUsuarios = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioPermissaoCadastroUsuarios"];
        _usuarioSemPerfilNemPermissao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioSemPerfilNemPermissao"];
        _usuarioAlterarStatus = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAlterarStatus"];
        _usuarioAlterarSeusDados = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioAlterarSeusDados"];
        _usuarioTrocarOrganizacao = (FfkApi.Domain.Entities.Usuario)entidades["UsuarioTrocarOrganizacao"];
    }

    [Test]
    [TestCase("Ativo", "")]
    [TestCase("Ausente", null)]
    [TestCase("Ativo", "   ")]
    [TestCase("Ausente", "usuario")]
    public async Task Sucesso_Alterar_Seus_Dados(string status, string? nomeOrganizacao)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAlterarSeusDados.Id);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = _usuarioAlterarSeusDados.Id.ToString();
        request.Organizacao = nomeOrganizacao == "usuario" ? _usuarioSemPerfilNemPermissao.Organizacao.Nome : nomeOrganizacao;
        request.Status = status;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaPermitidaAlterarStatusDeOutroUsuario))]
    public async Task Sucesso_Administrador(string status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = _usuarioAlterarStatus.Id.ToString();
        request.Organizacao = new Random().Next(2) == 0 ? "Nova" : "FfkApi";
        request.Status = status;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaPermitidaAlterarStatusDeOutroUsuario))]
    public async Task Sucesso_Usuario_Com_Permissao(string status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioPermissaoCadastroUsuarios.Id);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = _usuarioAlterarSeusDados.Id.ToString();
        request.Organizacao = new Random().Next(2) == 0 ? "Nova" : "FfkApi";
        request.Status = status;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Sucesso_Cpf_Existe_Em_Outra_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioTrocarOrganizacao);
        request.Organizacao = _usuarioSemPerfilNemPermissao.Organizacao.Nome == "FfkApi" ? "Nova" : "FfkApi";
        request.Cpf = _usuarioSemPerfilNemPermissao.Cpf;

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao_Alterar_Sua_Organizacao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Organizacao = _usuarioSemPerfilNemPermissao.Organizacao.Nome == "FfkApi" ? "Nova" : "FfkApi";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO_ALTERAR_ORGANIZACAO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Usuario_Sem_Permissao()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioPermissaoCadastroUsuarios);

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("SEM_PERMISSAO").Replace("{permissao}", "Cadastro de Usuários");

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Id = id;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Id = Guid.NewGuid().ToString();

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request);

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request, "TokenInvalido");

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request, token);

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

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var dadosDaResposta = await HttpResponseUtil.PegarDadosDaResposta(response);

        var erros = dadosDaResposta.RootElement.GetProperty("mensagensDeErro").EnumerateArray();

        var mensagemEsperada = MessagesException.GetString("TOKEN_EXPIRADO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
        Assert.That(dadosDaResposta.RootElement.GetProperty("tokenEstaExpirado").GetBoolean(), Is.True);
    }

    [Test]
    public async Task Erro_Email_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Email = _usuarioPermissaoCadastroUsuarios.Email;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAIL_JA_EXISTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Cpf_Ja_Existe()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Cpf = _usuarioPermissaoCadastroUsuarios.Cpf;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("CPF_JA_EXISTE");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("        ")]
    public async Task Erro_Email_Vazio(string? email)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Email = email;

        var response = await HttpHelper.DoPut(_baseUrlUsuario, cancellationToken, request, token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("EMAIL_VAZIO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_Organizacao_Nao_Encontrada()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Organizacao = "OrganizacaoInexistente";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("ORGANIZACAO_NAO_ENCONTRADA");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }


    [Test, TestCaseSource(typeof(StatusAoAlterarStatusUsuarioInlineData), nameof(StatusAoAlterarStatusUsuarioInlineData.ListaInvalidaAlterarStatusDeOutroUsuario))]
    public async Task Erro_Alterar_Status_Invalido_Outro_Usuario(string? status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Status = status;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var listaValida = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarStatusDeOutroUsuario().ListaSepadadaPorVirgula();
        var mensagemEsperada = MessagesException.GetString("STATUS_INVALIDO").Replace("{ValoresPossiveis}", listaValida);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    [TestCase("Suspenso")]
    [TestCase("Inativo")]
    [TestCase("StatusInvalido")]
    public async Task Erro_Alterar_Status_Invalido_Seu_Usuario(string status)
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioSemPerfilNemPermissao.Id);

        var request = RequestAlterarUsuarioBuilder.Build(_usuarioSemPerfilNemPermissao);
        request.Status = status;

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var listaValida = FfkApi.Domain.Entities.Usuario.StatusPermitidosAoAlterarSeuProprioStatus().ListaSepadadaPorVirgula();
        var mensagemEsperada = MessagesException.GetString("STATUS_INVALIDO").Replace("{ValoresPossiveis}", listaValida);

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }

    [Test]
    public async Task Erro_App_Token_Ausente()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        var token = GeradorTokenUsuarioBuilder.Build().Gerar(_usuarioAdministrador.Id);

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = _usuarioSemPerfilNemPermissao.Id.ToString();
        request.Organizacao = new Random().Next(2) == 0 ? "Nova" : "FfkApi";
        request.Status = "Ativo";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token, addAppToken: false);

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

        var request = RequestAlterarUsuarioBuilder.Build();
        request.Id = _usuarioSemPerfilNemPermissao.Id.ToString();
        request.Organizacao = new Random().Next(2) == 0 ? "Nova" : "FfkApi";
        request.Status = "Ativo";

        var response = await HttpHelper.DoPut(url: _baseUrlUsuario, cancellationToken: cancellationToken, request: request, token: token, appToken: "app-token-invalido");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        var erros = await HttpResponseUtil.PegarMensagensDeErro(response);

        var mensagemEsperada = MessagesException.GetString("TOKEN_APLICACAO_INVALIDO");

        Assert.That(erros.Count(), Is.EqualTo(1));
        Assert.That(erros.FirstOrDefault().GetString(), Is.EqualTo(mensagemEsperada));
    }
}
